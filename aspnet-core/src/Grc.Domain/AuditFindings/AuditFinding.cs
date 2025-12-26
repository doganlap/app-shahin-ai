using System;
using System.Collections.Generic;
using Grc.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.AuditFindings;

public class Audit : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string AuditId { get; private set; }
    public string Title { get; private set; }
    public string TitleAr { get; private set; }
    public AuditType Type { get; private set; }
    public AuditStatus Status { get; private set; }
    public Guid? FrameworkId { get; private set; }
    public string Scope { get; private set; }
    public string Objectives { get; private set; }
    public string AuditorName { get; private set; }
    public string AuditorOrganization { get; private set; }
    public string AuditorEmail { get; private set; }
    public string LeadAuditor { get; private set; }
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public DateTime? ReportDate { get; private set; }
    public string AuditOpinion { get; private set; }
    public string ExecutiveSummary { get; private set; }
    public int TotalFindings { get; private set; }
    public int CriticalFindings { get; private set; }
    public int HighFindings { get; private set; }
    public int MediumFindings { get; private set; }
    public int LowFindings { get; private set; }
    public DateTime? RemediationDeadline { get; private set; }
    public string Notes { get; private set; }

    public virtual ICollection<AuditFinding> Findings { get; private set; }

    protected Audit() { }

    public Audit(
        Guid id,
        string auditId,
        string title,
        AuditType type,
        DateTime plannedStartDate,
        DateTime plannedEndDate,
        Guid? tenantId = null)
        : base(id)
    {
        AuditId = auditId;
        Title = title;
        Type = type;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        TenantId = tenantId;
        Status = AuditStatus.Planned;
        Findings = new List<AuditFinding>();
    }

    public void SetFramework(Guid frameworkId) => FrameworkId = frameworkId;

    public void SetScope(string scope, string objectives)
    {
        Scope = scope;
        Objectives = objectives;
    }

    public void SetAuditor(string name, string organization, string email, string leadAuditor)
    {
        AuditorName = name;
        AuditorOrganization = organization;
        AuditorEmail = email;
        LeadAuditor = leadAuditor;
    }

    public void Start(DateTime actualStartDate)
    {
        ActualStartDate = actualStartDate;
        Status = AuditStatus.InProgress;
    }

    public void Complete(DateTime actualEndDate, string opinion, string executiveSummary)
    {
        ActualEndDate = actualEndDate;
        AuditOpinion = opinion;
        ExecutiveSummary = executiveSummary;
        Status = AuditStatus.Completed;
    }

    public void IssueReport(DateTime reportDate, DateTime remediationDeadline)
    {
        ReportDate = reportDate;
        RemediationDeadline = remediationDeadline;
        Status = AuditStatus.ReportIssued;
    }

    public void UpdateFindingCounts(int critical, int high, int medium, int low)
    {
        CriticalFindings = critical;
        HighFindings = high;
        MediumFindings = medium;
        LowFindings = low;
        TotalFindings = critical + high + medium + low;
    }
}

public class AuditFinding : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid AuditId { get; private set; }
    public string FindingId { get; private set; }
    public string Title { get; private set; }
    public string TitleAr { get; private set; }
    public Guid? ControlId { get; private set; }
    public string ControlReference { get; private set; }
    public string Description { get; private set; }
    public string DescriptionAr { get; private set; }
    public FindingSeverity Severity { get; private set; }
    public FindingStatus Status { get; private set; }
    public string Condition { get; private set; }
    public string Criteria { get; private set; }
    public string Cause { get; private set; }
    public string Effect { get; private set; }
    public string Recommendation { get; private set; }
    public string RecommendationAr { get; private set; }
    public string ManagementResponse { get; private set; }
    public string ResponsiblePerson { get; private set; }
    public string ResponsibleEmail { get; private set; }
    public DateTime? TargetRemediationDate { get; private set; }
    public DateTime? ActualRemediationDate { get; private set; }
    public string RemediationEvidence { get; private set; }
    public string VerifiedBy { get; private set; }
    public DateTime? VerifiedDate { get; private set; }
    public bool IsRepeat { get; private set; }
    public string PreviousFindingReference { get; private set; }
    public string Notes { get; private set; }

    protected AuditFinding() { }

    public AuditFinding(
        Guid id,
        Guid auditId,
        string findingId,
        string title,
        FindingSeverity severity,
        Guid? tenantId = null)
        : base(id)
    {
        AuditId = auditId;
        FindingId = findingId;
        Title = title;
        Severity = severity;
        TenantId = tenantId;
        Status = FindingStatus.Open;
    }

    public void SetControlReference(Guid? controlId, string controlReference)
    {
        ControlId = controlId;
        ControlReference = controlReference;
    }

    public void SetDescription(string description, string descriptionAr = null)
    {
        Description = description;
        DescriptionAr = descriptionAr;
    }

    public void SetArabicTitle(string titleAr) => TitleAr = titleAr;

    public void SetFindingDetails(string condition, string criteria, string cause, string effect)
    {
        Condition = condition;
        Criteria = criteria;
        Cause = cause;
        Effect = effect;
    }

    public void SetRecommendation(string recommendation, string recommendationAr = null)
    {
        Recommendation = recommendation;
        RecommendationAr = recommendationAr;
    }

    public void SetManagementResponse(string response, string responsiblePerson, string email, DateTime targetDate)
    {
        ManagementResponse = response;
        ResponsiblePerson = responsiblePerson;
        ResponsibleEmail = email;
        TargetRemediationDate = targetDate;
        Status = FindingStatus.ActionPlanned;
    }

    public void StartRemediation()
    {
        Status = FindingStatus.InRemediation;
    }

    public void SubmitRemediation(DateTime actualDate, string evidence)
    {
        ActualRemediationDate = actualDate;
        RemediationEvidence = evidence;
        Status = FindingStatus.PendingVerification;
    }

    public void Verify(string verifiedBy, DateTime verifiedDate)
    {
        VerifiedBy = verifiedBy;
        VerifiedDate = verifiedDate;
        Status = FindingStatus.Closed;
    }

    public void Reject(string reason)
    {
        Notes = reason;
        Status = FindingStatus.InRemediation;
    }

    public void MarkAsRepeat(string previousReference)
    {
        IsRepeat = true;
        PreviousFindingReference = previousReference;
    }

    public void Reopen(string reason)
    {
        Status = FindingStatus.Reopened;
        Notes = reason;
    }
}
