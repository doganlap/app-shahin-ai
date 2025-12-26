using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Grc.Enums;

namespace Grc.ActionPlan;

/// <summary>
/// Individual action item within an action plan
/// </summary>
public class ActionItem : FullAuditedEntity<Guid>
{
    public Guid ActionPlanId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid AssignedToUserId { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public WorkflowStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public string CompletionNotes { get; private set; }
    
    protected ActionItem() { }
    
    public ActionItem(Guid id, Guid actionPlanId, string title, string description, Guid assignedToUserId, DateTime? dueDate = null)
        : base(id)
    {
        ActionPlanId = actionPlanId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Description = description;
        AssignedToUserId = assignedToUserId;
        DueDate = dueDate;
        Status = WorkflowStatus.Pending;
        Priority = Priority.Medium;
    }
    
    public void Start()
    {
        Status = WorkflowStatus.InProgress;
    }
    
    public void Complete(string completionNotes = null)
    {
        Status = WorkflowStatus.Approved;
        CompletedDate = DateTime.UtcNow;
        CompletionNotes = completionNotes;
    }
    
    public void SetPriority(Priority priority)
    {
        Priority = priority;
    }
    
    public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != WorkflowStatus.Approved;
}

