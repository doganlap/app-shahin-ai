using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.ValueObjects;

namespace Grc.Assessment.Domain.Issues;

/// <summary>
/// Issue entity for tracking findings, vulnerabilities, and non-compliance items
/// </summary>
public class Issue : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string IssueCode { get; private set; }
    public LocalizedString Title { get; private set; }
    public LocalizedString Description { get; private set; }
    public IssueType Type { get; private set; }
    public IssueSeverity Severity { get; private set; }
    public IssueStatus Status { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public Guid? SourceAssessmentId { get; private set; }
    public Guid? SourceControlId { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? ResolvedDate { get; private set; }
    
    protected Issue() { }
    
    public Issue(Guid id, string issueCode, LocalizedString title, IssueType type, IssueSeverity severity)
        : base(id)
    {
        IssueCode = Check.NotNullOrWhiteSpace(issueCode, nameof(issueCode), maxLength: 30);
        Title = Check.NotNull(title, nameof(title));
        Type = type;
        Severity = severity;
        Status = IssueStatus.Open;
    }
    
    public void SetDescription(LocalizedString description)
    {
        Description = description;
    }
    
    public void AssignTo(Guid userId)
    {
        AssignedToUserId = userId;
        if (Status == IssueStatus.Open)
        {
            Status = IssueStatus.InProgress;
        }
    }
    
    public void SetDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
    }
    
    public void LinkToAssessment(Guid assessmentId)
    {
        SourceAssessmentId = assessmentId;
    }
    
    public void LinkToControl(Guid controlId)
    {
        SourceControlId = controlId;
    }
    
    public void Resolve()
    {
        Status = IssueStatus.Resolved;
        ResolvedDate = DateTime.UtcNow;
    }
    
    public void Close()
    {
        Status = IssueStatus.Closed;
    }
    
    public void Reopen()
    {
        Status = IssueStatus.Open;
        ResolvedDate = null;
    }
    
    public void PutOnHold()
    {
        Status = IssueStatus.OnHold;
    }
    
    public void Reject()
    {
        Status = IssueStatus.Rejected;
    }
}
