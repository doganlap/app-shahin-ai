using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Audit.Domain.Audits;

/// <summary>
/// Audit entity (internal/external audit)
/// </summary>
public class Audit : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string AuditCode { get; private set; }
    public LocalizedString Title { get; private set; }
    public LocalizedString Description { get; private set; }
    public string AuditType { get; private set; } // Internal, External, Regulatory
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid LeadAuditorId { get; private set; }
    public List<Guid> AuditorIds { get; private set; }
    public WorkflowStatus Status { get; private set; }
    public string Scope { get; private set; }
    public string Objectives { get; private set; }
    
    public ICollection<AuditFinding> Findings { get; private set; }
    
    protected Audit() { }
    
    public Audit(Guid id, string auditCode, LocalizedString title, string auditType, DateTime startDate, DateTime endDate, Guid leadAuditorId)
        : base(id)
    {
        AuditCode = Check.NotNullOrWhiteSpace(auditCode, nameof(auditCode));
        Title = Check.NotNull(title, nameof(title));
        AuditType = auditType;
        StartDate = startDate;
        EndDate = endDate;
        LeadAuditorId = leadAuditorId;
        Status = WorkflowStatus.Pending;
        AuditorIds = new List<Guid>();
        Findings = new Collection<AuditFinding>();
    }
    
    public void Start()
    {
        Status = WorkflowStatus.InProgress;
    }
    
    public void Complete()
    {
        Status = WorkflowStatus.Approved;
    }
    
    public AuditFinding AddFinding(string findingCode, LocalizedString title, RiskLevel severity)
    {
        var finding = new AuditFinding(
            Guid.NewGuid(),
            Id,
            findingCode,
            title,
            severity);
        
        Findings.Add(finding);
        return finding;
    }
}

