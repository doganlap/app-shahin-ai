using System;
using Volo.Abp.Domain.Entities;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Audit.Domain.Audits;

/// <summary>
/// Audit finding
/// </summary>
public class AuditFinding : Entity<Guid>
{
    public Guid AuditId { get; private set; }
    public string FindingCode { get; private set; }
    public LocalizedString Title { get; private set; }
    public LocalizedString Description { get; private set; }
    public RiskLevel Severity { get; private set; }
    public string Status { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public DateTime? TargetResolutionDate { get; private set; }
    public DateTime? ResolvedDate { get; private set; }
    public string ResolutionNotes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    protected AuditFinding() { }
    
    public AuditFinding(Guid id, Guid auditId, string findingCode, LocalizedString title, RiskLevel severity)
    {
        Id = id;
        AuditId = auditId;
        FindingCode = findingCode;
        Title = title;
        Severity = severity;
        Status = "Open";
        CreatedAt = DateTime.UtcNow;
    }
    
    public void AssignTo(Guid userId, DateTime? targetDate = null)
    {
        AssignedToUserId = userId;
        TargetResolutionDate = targetDate;
    }
    
    public void Resolve(string resolutionNotes)
    {
        Status = "Resolved";
        ResolvedDate = DateTime.UtcNow;
        ResolutionNotes = resolutionNotes;
    }
}

