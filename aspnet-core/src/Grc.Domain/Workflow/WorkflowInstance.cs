using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.Enums;

namespace Grc.Workflow;

/// <summary>
/// Workflow instance (execution of a workflow definition)
/// </summary>
public class WorkflowInstance : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid WorkflowDefinitionId { get; private set; }
    public string CurrentActivityId { get; private set; }
    public WorkflowStatus Status { get; private set; }
    public Dictionary<string, object> Variables { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string CompletedByUserId { get; private set; }
    
    public ICollection<WorkflowTask> Tasks { get; private set; }
    
    protected WorkflowInstance() { }
    
    public WorkflowInstance(Guid id, Guid workflowDefinitionId, Guid? tenantId, Dictionary<string, object> variables)
        : base(id)
    {
        WorkflowDefinitionId = workflowDefinitionId;
        TenantId = tenantId;
        Variables = variables ?? new Dictionary<string, object>();
        Status = WorkflowStatus.Pending;
        Tasks = new Collection<WorkflowTask>();
    }
    
    public void Start()
    {
        if (Status != WorkflowStatus.Pending)
            throw new InvalidOperationException("Workflow can only be started from Pending status");
        
        Status = WorkflowStatus.InProgress;
        StartedAt = DateTime.UtcNow;
    }
    
    public void Complete(string userId = null)
    {
        Status = WorkflowStatus.Approved;
        CompletedAt = DateTime.UtcNow;
        CompletedByUserId = userId;
    }
    
    public void Reject(string reason, string userId = null)
    {
        Status = WorkflowStatus.Rejected;
        CompletedAt = DateTime.UtcNow;
        CompletedByUserId = userId;
        Variables["RejectionReason"] = reason;
    }
    
    public void MoveToActivity(string activityId)
    {
        CurrentActivityId = activityId;
    }
    
    public WorkflowTask CreateTask(string taskName, string assignedToUserId, Dictionary<string, object> taskData = null)
    {
        var task = new WorkflowTask(
            Guid.NewGuid(),
            Id,
            taskName,
            assignedToUserId,
            taskData);
        
        Tasks.Add(task);
        return task;
    }
}

