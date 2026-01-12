using System;
using System.Collections.Generic;
using Grc.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.ActionItems;

public class ActionItem : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string ActionId { get; private set; }
    public Guid? GapId { get; private set; }
    public Guid? RiskId { get; private set; }
    public Guid? AuditFindingId { get; private set; }
    public Guid? AssessmentId { get; private set; }
    public Guid? ControlId { get; private set; }
    public ActionItemSource Source { get; private set; }
    public string Title { get; private set; }
    public string TitleAr { get; private set; }
    public string Description { get; private set; }
    public string DescriptionAr { get; private set; }
    public ActionItemType Type { get; private set; }
    public ActionItemPriority Priority { get; private set; }
    public ActionItemStatus Status { get; private set; }
    public string AssignedTo { get; private set; }
    public string AssignedToEmail { get; private set; }
    public string AssignedBy { get; private set; }
    public DateTime? AssignedDate { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public int EstimatedHours { get; private set; }
    public int ActualHours { get; private set; }
    public decimal? EstimatedCost { get; private set; }
    public decimal? ActualCost { get; private set; }
    public int ProgressPercentage { get; private set; }
    public string CompletionEvidence { get; private set; }
    public string VerifiedBy { get; private set; }
    public DateTime? VerifiedDate { get; private set; }
    public string Notes { get; private set; }
    public bool IsOverdue { get; private set; }

    public virtual ICollection<ActionItemComment> Comments { get; private set; }
    public virtual ICollection<ActionItemAttachment> Attachments { get; private set; }

    protected ActionItem() { }

    public ActionItem(
        Guid id,
        string actionId,
        string title,
        ActionItemType type,
        ActionItemPriority priority,
        ActionItemSource source,
        Guid? tenantId = null)
        : base(id)
    {
        ActionId = actionId;
        Title = title;
        Type = type;
        Priority = priority;
        Source = source;
        TenantId = tenantId;
        Status = ActionItemStatus.NotStarted;
        ProgressPercentage = 0;
        Comments = new List<ActionItemComment>();
        Attachments = new List<ActionItemAttachment>();
    }

    public void SetSourceReference(Guid? gapId, Guid? riskId, Guid? auditFindingId, Guid? assessmentId, Guid? controlId)
    {
        GapId = gapId;
        RiskId = riskId;
        AuditFindingId = auditFindingId;
        AssessmentId = assessmentId;
        ControlId = controlId;
    }

    public void SetDescription(string description, string descriptionAr = null)
    {
        Description = description;
        DescriptionAr = descriptionAr;
    }

    public void SetArabicTitle(string titleAr) => TitleAr = titleAr;

    public void Assign(string assignedTo, string assignedToEmail, string assignedBy, DateTime dueDate)
    {
        AssignedTo = assignedTo;
        AssignedToEmail = assignedToEmail;
        AssignedBy = assignedBy;
        AssignedDate = DateTime.UtcNow;
        DueDate = dueDate;
    }

    public void SetEstimates(int hours, decimal? cost)
    {
        EstimatedHours = hours;
        EstimatedCost = cost;
    }

    public void Start(DateTime startDate)
    {
        StartDate = startDate;
        Status = ActionItemStatus.InProgress;
    }

    public void UpdateProgress(int percentage)
    {
        ProgressPercentage = Math.Min(100, Math.Max(0, percentage));
        if (ProgressPercentage == 100 && Status != ActionItemStatus.Completed)
        {
            Status = ActionItemStatus.PendingVerification;
        }
    }

    public void Complete(DateTime completedDate, string evidence, int actualHours, decimal? actualCost)
    {
        CompletedDate = completedDate;
        CompletionEvidence = evidence;
        ActualHours = actualHours;
        ActualCost = actualCost;
        ProgressPercentage = 100;
        Status = ActionItemStatus.PendingVerification;
    }

    public void Verify(string verifiedBy, DateTime verifiedDate)
    {
        VerifiedBy = verifiedBy;
        VerifiedDate = verifiedDate;
        Status = ActionItemStatus.Completed;
    }

    public void Reject(string reason)
    {
        Notes = reason;
        Status = ActionItemStatus.InProgress;
        ProgressPercentage = 80;
    }

    public void Cancel(string reason)
    {
        Notes = reason;
        Status = ActionItemStatus.Cancelled;
    }

    public void MarkOverdue() => IsOverdue = true;
    public void ClearOverdue() => IsOverdue = false;
}

public class ActionItemComment : FullAuditedEntity<Guid>
{
    public Guid ActionItemId { get; private set; }
    public string Comment { get; private set; }
    public string CommentBy { get; private set; }
    public string CommentByEmail { get; private set; }

    protected ActionItemComment() { }

    public ActionItemComment(Guid id, Guid actionItemId, string comment, string commentBy, string commentByEmail)
        : base(id)
    {
        ActionItemId = actionItemId;
        Comment = comment;
        CommentBy = commentBy;
        CommentByEmail = commentByEmail;
    }
}

public class ActionItemAttachment : FullAuditedEntity<Guid>
{
    public Guid ActionItemId { get; private set; }
    public string FileName { get; private set; }
    public string FileType { get; private set; }
    public long FileSize { get; private set; }
    public string BlobPath { get; private set; }
    public string Description { get; private set; }

    protected ActionItemAttachment() { }

    public ActionItemAttachment(Guid id, Guid actionItemId, string fileName, string fileType, long fileSize, string blobPath)
        : base(id)
    {
        ActionItemId = actionItemId;
        FileName = fileName;
        FileType = fileType;
        FileSize = fileSize;
        BlobPath = blobPath;
    }
}
