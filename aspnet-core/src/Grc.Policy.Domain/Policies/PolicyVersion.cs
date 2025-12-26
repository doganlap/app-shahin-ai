using System;
using Volo.Abp.Domain.Entities;

namespace Grc.Policy.Domain.Policies;

/// <summary>
/// Policy version
/// </summary>
public class PolicyVersion : Entity<Guid>
{
    public Guid PolicyId { get; private set; }
    public string VersionNumber { get; private set; }
    public string Content { get; private set; }
    public string ChangeSummary { get; private set; }
    public bool IsCurrentVersion { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    
    protected PolicyVersion() { }
    
    public PolicyVersion(Guid id, Guid policyId, string versionNumber, string content, string changeSummary)
    {
        Id = id;
        PolicyId = policyId;
        VersionNumber = versionNumber;
        Content = content;
        ChangeSummary = changeSummary;
        IsCurrentVersion = false;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void MarkAsCurrent()
    {
        IsCurrentVersion = true;
    }
}

