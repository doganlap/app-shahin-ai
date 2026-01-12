using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Grc.Enums;
using Grc.Integration;
using Grc.Workflow;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Grc.Workflow.Application;

/// <summary>
/// Workflow engine for executing BPMN-style workflows with n8n + Ollama AI integration
/// </summary>
public class WorkflowEngine : ITransientDependency
{
    private readonly IRepository<WorkflowDefinition, Guid> _definitionRepository;
    private readonly IRepository<WorkflowInstance, Guid> _instanceRepository;
    private readonly IN8nClientService _n8nClient;
    private readonly IAiOrchestrationService _aiOrchestration;
    private readonly ILogger<WorkflowEngine> _logger;

    public WorkflowEngine(
        IRepository<WorkflowDefinition, Guid> definitionRepository,
        IRepository<WorkflowInstance, Guid> instanceRepository,
        IN8nClientService n8nClient,
        IAiOrchestrationService aiOrchestration,
        ILogger<WorkflowEngine> logger)
    {
        _definitionRepository = definitionRepository;
        _instanceRepository = instanceRepository;
        _n8nClient = n8nClient;
        _aiOrchestration = aiOrchestration;
        _logger = logger;
    }

    /// <summary>
    /// Start a workflow instance
    /// </summary>
    public async Task<WorkflowInstance> StartWorkflowAsync(
        Guid workflowDefinitionId,
        Guid? tenantId,
        Dictionary<string, object> inputVariables = null)
    {
        var definition = await _definitionRepository.GetAsync(workflowDefinitionId);
        
        if (definition.Status != WorkflowStatus.InProgress)
        {
            throw new InvalidOperationException($"Workflow definition {definition.Name} is not active");
        }

        var instance = definition.CreateInstance(tenantId, inputVariables);
        instance.Start();
        
        await _instanceRepository.InsertAsync(instance);

        // Trigger n8n workflow for AI-powered task creation
        await TriggerN8nWorkflowAsync(instance, definition, inputVariables);

        _logger.LogInformation("Started workflow instance {InstanceId} for definition {DefinitionId}",
            instance.Id, workflowDefinitionId);

        return instance;
    }

    /// <summary>
    /// Trigger n8n workflow for AI-powered automation
    /// </summary>
    private async Task TriggerN8nWorkflowAsync(
        WorkflowInstance instance,
        WorkflowDefinition definition,
        Dictionary<string, object> inputVariables)
    {
        try
        {
            var isN8nAvailable = await _n8nClient.IsN8nAvailableAsync();
            if (!isN8nAvailable)
            {
                _logger.LogWarning("n8n is not available, skipping workflow automation");
                return;
            }

            // Determine webhook path based on workflow type
            var webhookPath = DetermineWebhookPath(definition.Name.En, definition.Category);

            var payload = new
            {
                workflowInstanceId = instance.Id,
                workflowName = definition.Name.En,
                category = definition.Category,
                inputVariables = inputVariables ?? new Dictionary<string, object>(),
                timestamp = DateTime.UtcNow,
                tenantId = instance.TenantId
            };

            var response = await _n8nClient.TriggerWorkflowAsync(webhookPath, payload);

            if (response.Success)
            {
                _logger.LogInformation(
                    "n8n workflow triggered successfully for instance {InstanceId}, response: {Response}",
                    instance.Id, response.Analysis ?? response.RawResponse);

                // Store AI recommendations in workflow variables
                if (response.Recommendations?.Length > 0)
                {
                    instance.Variables["aiRecommendations"] = JsonSerializer.Serialize(response.Recommendations);
                }
                if (response.ActionItems?.Length > 0)
                {
                    instance.Variables["aiActionItems"] = JsonSerializer.Serialize(response.ActionItems);
                }

                await _instanceRepository.UpdateAsync(instance);
            }
            else
            {
                _logger.LogWarning("n8n workflow failed for instance {InstanceId}: {Error}",
                    instance.Id, response.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering n8n workflow for instance {InstanceId}", instance.Id);
        }
    }

    /// <summary>
    /// Determine the appropriate n8n webhook path based on workflow type
    /// </summary>
    private string DetermineWebhookPath(string workflowName, string category)
    {
        var normalizedName = workflowName?.ToLowerInvariant() ?? "";
        var normalizedCategory = category?.ToLowerInvariant() ?? "";

        if (normalizedName.Contains("compliance") || normalizedCategory.Contains("compliance"))
            return "grc-compliance-check";

        if (normalizedName.Contains("risk") || normalizedCategory.Contains("risk"))
            return "grc-risk-analysis";

        if (normalizedName.Contains("audit") || normalizedCategory.Contains("audit"))
            return "grc-audit-workflow";

        if (normalizedName.Contains("assessment") || normalizedCategory.Contains("assessment"))
            return "grc-assessment-workflow";

        return "grc-generic-workflow";
    }

    /// <summary>
    /// Complete a workflow task and move to next activity
    /// </summary>
    public async Task CompleteTaskAsync(Guid taskId, string userId, Dictionary<string, object> outputData = null)
    {
        var instance = await _instanceRepository.GetAsync(i => i.Tasks.Any(t => t.Id == taskId));
        var task = instance.Tasks.FirstOrDefault(t => t.Id == taskId);
        
        if (task == null)
        {
            throw new InvalidOperationException($"Task {taskId} not found");
        }

        task.Complete(userId);

        // Use AI to evaluate next steps and provide recommendations
        await EvaluateNextStepsWithAiAsync(instance, task, outputData);

        await _instanceRepository.UpdateAsync(instance);

        _logger.LogInformation("Completed task {TaskId} in workflow instance {InstanceId}",
            taskId, instance.Id);
    }

    /// <summary>
    /// Get active tasks for a user
    /// </summary>
    public async Task<List<WorkflowTask>> GetUserTasksAsync(string userId)
    {
        var instances = await _instanceRepository.GetListAsync(i => 
            i.Status == WorkflowStatus.InProgress &&
            i.Tasks.Any(t => t.AssignedToUserId == userId && t.Status == WorkflowStatus.Pending));
        
        return instances.SelectMany(i => i.Tasks)
            .Where(t => t.AssignedToUserId == userId && t.Status == WorkflowStatus.Pending)
            .ToList();
    }

    /// <summary>
    /// Get workflow instance status
    /// </summary>
    public async Task<WorkflowInstance> GetInstanceAsync(Guid instanceId)
    {
        return await _instanceRepository.GetAsync(instanceId);
    }

    /// <summary>
    /// Use AI to evaluate next steps after task completion
    /// </summary>
    private async Task EvaluateNextStepsWithAiAsync(
        WorkflowInstance instance,
        WorkflowTask completedTask,
        Dictionary<string, object> outputData)
    {
        try
        {
            var pendingTasks = instance.Tasks.Count(t => t.Status == WorkflowStatus.Pending);
            var completedTasks = instance.Tasks.Count(t => t.Status == WorkflowStatus.Completed);

            // If all tasks are complete, get AI summary
            if (pendingTasks == 0)
            {
                var result = await _aiOrchestration.ChatAsync(
                    $"Workflow '{instance.Id}' has completed all {completedTasks} tasks. " +
                    $"Provide a brief summary and any recommended follow-up actions.",
                    $"Workflow variables: {JsonSerializer.Serialize(instance.Variables)}"
                );

                if (result.Success)
                {
                    instance.Variables["aiCompletionSummary"] = result.Response;
                    instance.Complete();
                    _logger.LogInformation("Workflow {InstanceId} completed with AI summary", instance.Id);
                }
            }
            else
            {
                // Notify n8n about task completion for potential automation
                var isN8nAvailable = await _n8nClient.IsN8nAvailableAsync();
                if (isN8nAvailable)
                {
                    await _n8nClient.TriggerWorkflowAsync("grc-task-completed", new
                    {
                        workflowInstanceId = instance.Id,
                        completedTaskId = completedTask.Id,
                        completedTaskName = completedTask.TaskName,
                        pendingTasksCount = pendingTasks,
                        outputData = outputData
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI evaluation failed for task {TaskId}, continuing without AI", completedTask.Id);
        }
    }

    /// <summary>
    /// Get AI recommendations for a workflow
    /// </summary>
    public async Task<AiOrchestrationResult> GetAiRecommendationsAsync(Guid instanceId)
    {
        var instance = await _instanceRepository.GetAsync(instanceId);

        return await _aiOrchestration.ChatAsync(
            $"Analyze the current state of workflow '{instance.Id}' and provide recommendations. " +
            $"Current status: {instance.Status}, Tasks: {instance.Tasks.Count}",
            JsonSerializer.Serialize(instance.Variables)
        );
    }

    /// <summary>
    /// Get orchestrator status (n8n + Ollama availability)
    /// </summary>
    public async Task<OrchestratorStatus> GetOrchestratorStatusAsync()
    {
        return await _aiOrchestration.GetStatusAsync();
    }
}

