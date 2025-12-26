using System;
using System.Collections.Generic;
using Grc.Enums;
using Volo.Abp.Domain.Entities;

namespace Grc.Workflow;

/// <summary>
/// Task within a workflow instance
/// </summary>
public class WorkflowTask : Entity<Guid>
{
    public Guid WorkflowInstanceId { get; private set; }
    public string TaskName { get; private set; }
    public string AssignedToUserId { get; private set; }
    public WorkflowStatus Status { get; private set; }
    public Dictionary<string, object> TaskData { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string CompletedByUserId { get; private set; }
    public string Comments { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    protected WorkflowTask() { }
    
    public WorkflowTask(Guid id, Guid workflowInstanceId, string taskName, string assignedToUserId, Dictionary<string, object> taskData = null)
    {
        Id = id;
        WorkflowInstanceId = workflowInstanceId;
        TaskName = taskName;
        AssignedToUserId = assignedToUserId;
        Status = WorkflowStatus.Pending;
        TaskData = taskData ?? new Dictionary<string, object>();
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Complete(string userId, string comments = null)
    {
        Status = WorkflowStatus.Approved;
        CompletedAt = DateTime.UtcNow;
        CompletedByUserId = userId;
        Comments = comments;
    }
    
    public void Reject(string userId, string reason)
    {
        Status = WorkflowStatus.Rejected;
        CompletedAt = DateTime.UtcNow;
        CompletedByUserId = userId;
        Comments = reason;
    }
}

