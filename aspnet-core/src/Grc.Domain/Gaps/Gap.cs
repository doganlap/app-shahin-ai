using System;
using System.Collections.Generic;
using Grc.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Gaps;

public class Gap : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string GapId { get; private set; }
    public Guid? AssessmentId { get; private set; }
    public Guid? ControlId { get; private set; }
    public Guid? FrameworkId { get; private set; }
    public string ControlReference { get; private set; }
    public string Title { get; private set; }
    public string TitleAr { get; private set; }
    public string CurrentState { get; private set; }
    public string RequiredState { get; private set; }
    public string GapDescription { get; private set; }
    public string GapDescriptionAr { get; private set; }
    public GapSeverity Severity { get; private set; }
    public GapPriority Priority { get; private set; }
    public GapStatus Status { get; private set; }
    public string RootCause { get; private set; }
    public string BusinessImpact { get; private set; }
    public int RiskScore { get; private set; }
    public int EstimatedEffortHours { get; private set; }
    public decimal? EstimatedCost { get; private set; }
    public string RemediationOwner { get; private set; }
    public string RemediationOwnerEmail { get; private set; }
    public DateTime? TargetClosureDate { get; private set; }
    public DateTime? ActualClosureDate { get; private set; }
    public string ClosureEvidence { get; private set; }
    public string Notes { get; private set; }

    public virtual ICollection<GapRemediation> RemediationSteps { get; private set; }

    protected Gap() { }

    public Gap(
        Guid id,
        string gapId,
        string title,
        string controlReference,
        GapSeverity severity,
        Guid? tenantId = null)
        : base(id)
    {
        GapId = gapId;
        Title = title;
        ControlReference = controlReference;
        Severity = severity;
        TenantId = tenantId;
        Status = GapStatus.Open;
        Priority = GapPriority.Medium;
        RemediationSteps = new List<GapRemediation>();
    }

    public void SetAssessmentContext(Guid assessmentId, Guid? controlId, Guid? frameworkId)
    {
        AssessmentId = assessmentId;
        ControlId = controlId;
        FrameworkId = frameworkId;
    }

    public void SetGapDetails(string currentState, string requiredState, string gapDescription)
    {
        CurrentState = currentState;
        RequiredState = requiredState;
        GapDescription = gapDescription;
    }

    public void SetArabicContent(string titleAr, string gapDescriptionAr)
    {
        TitleAr = titleAr;
        GapDescriptionAr = gapDescriptionAr;
    }

    public void SetAnalysis(string rootCause, string businessImpact, int riskScore)
    {
        RootCause = rootCause;
        BusinessImpact = businessImpact;
        RiskScore = riskScore;
    }

    public void SetEstimates(int effortHours, decimal? cost)
    {
        EstimatedEffortHours = effortHours;
        EstimatedCost = cost;
    }

    public void AssignOwner(string owner, string email, DateTime targetDate)
    {
        RemediationOwner = owner;
        RemediationOwnerEmail = email;
        TargetClosureDate = targetDate;
    }

    public void UpdatePriority(GapPriority priority) => Priority = priority;
    public void UpdateStatus(GapStatus status) => Status = status;

    public void Close(DateTime closureDate, string evidence)
    {
        ActualClosureDate = closureDate;
        ClosureEvidence = evidence;
        Status = GapStatus.Closed;
    }

    public void Reopen(string reason)
    {
        ActualClosureDate = null;
        Status = GapStatus.Reopened;
        Notes = reason;
    }
}

public class GapRemediation : FullAuditedEntity<Guid>
{
    public Guid GapId { get; private set; }
    public int StepNumber { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string AssignedTo { get; private set; }
    public string AssignedToEmail { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public GapStatus Status { get; private set; }
    public string Evidence { get; private set; }
    public string Notes { get; private set; }

    protected GapRemediation() { }

    public GapRemediation(Guid id, Guid gapId, int stepNumber, string title, string description)
        : base(id)
    {
        GapId = gapId;
        StepNumber = stepNumber;
        Title = title;
        Description = description;
        Status = GapStatus.Open;
    }

    public void Assign(string assignedTo, string email, DateTime dueDate)
    {
        AssignedTo = assignedTo;
        AssignedToEmail = email;
        DueDate = dueDate;
    }

    public void Complete(DateTime completedDate, string evidence)
    {
        CompletedDate = completedDate;
        Evidence = evidence;
        Status = GapStatus.Closed;
    }
}
