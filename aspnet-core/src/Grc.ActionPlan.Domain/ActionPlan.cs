using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.Enums;

namespace Grc.ActionPlan;

/// <summary>
/// Action plan for remediation
/// </summary>
public class ActionPlan : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid? AssessmentId { get; private set; }
    public Guid? RiskId { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime TargetEndDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public WorkflowStatus Status { get; private set; }
    
    public ICollection<ActionItem> ActionItems { get; private set; }
    
    protected ActionPlan() { }
    
    public ActionPlan(Guid id, string name, Guid ownerUserId, DateTime startDate, DateTime targetEndDate)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        OwnerUserId = ownerUserId;
        StartDate = startDate;
        TargetEndDate = targetEndDate;
        Status = WorkflowStatus.Pending;
        ActionItems = new Collection<ActionItem>();
    }
    
    public ActionItem AddActionItem(string title, string description, Guid assignedToUserId, DateTime? dueDate = null)
    {
        var item = new ActionItem(
            Guid.NewGuid(),
            Id,
            title,
            description,
            assignedToUserId,
            dueDate);
        
        ActionItems.Add(item);
        return item;
    }
    
    public void Start()
    {
        Status = WorkflowStatus.InProgress;
    }
    
    public void Complete()
    {
        Status = WorkflowStatus.Approved;
        ActualEndDate = DateTime.UtcNow;
    }
    
    public void SetDescription(string description)
    {
        Description = description;
    }
    
    public void LinkToAssessment(Guid assessmentId)
    {
        AssessmentId = assessmentId;
    }
    
    public void LinkToRisk(Guid riskId)
    {
        RiskId = riskId;
    }
}

