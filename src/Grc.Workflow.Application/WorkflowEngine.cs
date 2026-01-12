using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Workflow;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace Grc.Workflow.Application;

/// <summary>
/// Workflow engine for executing BPMN-style workflows
/// </summary>
public class WorkflowEngine
{
    private readonly IRepository<WorkflowDefinition, Guid> _definitionRepository;
    private readonly IRepository<WorkflowInstance, Guid> _instanceRepository;
    private readonly ILogger<WorkflowEngine> _logger;

    public WorkflowEngine(
        IRepository<WorkflowDefinition, Guid> definitionRepository,
        IRepository<WorkflowInstance, Guid> instanceRepository,
        ILogger<WorkflowEngine> logger)
    {
        _definitionRepository = definitionRepository;
        _instanceRepository = instanceRepository;
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
        
        // TODO: Parse BPMN XML and create initial tasks
        // This would require a BPMN parser library
        
        _logger.LogInformation("Started workflow instance {InstanceId} for definition {DefinitionId}", 
            instance.Id, workflowDefinitionId);
        
        return instance;
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
        
        // TODO: Evaluate BPMN flow and move to next activity
        // This would require BPMN execution engine
        
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
}

