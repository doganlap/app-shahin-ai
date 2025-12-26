using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.ValueObjects;

namespace Grc.Policy.Domain.Policies;

/// <summary>
/// Policy entity
/// </summary>
public class Policy : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string PolicyCode { get; private set; }
    public LocalizedString Title { get; private set; }
    public LocalizedString Description { get; private set; }
    public string Category { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public Guid OwnerUserId { get; private set; }
    
    public ICollection<PolicyVersion> Versions { get; private set; }
    public ICollection<PolicyAttestation> Attestations { get; private set; }
    
    protected Policy() { }
    
    public Policy(Guid id, string policyCode, LocalizedString title, Guid ownerUserId, DateTime effectiveDate)
        : base(id)
    {
        PolicyCode = Check.NotNullOrWhiteSpace(policyCode, nameof(policyCode));
        Title = Check.NotNull(title, nameof(title));
        OwnerUserId = ownerUserId;
        EffectiveDate = effectiveDate;
        IsActive = true;
        Versions = new Collection<PolicyVersion>();
        Attestations = new Collection<PolicyAttestation>();
    }
    
    public PolicyVersion CreateVersion(string versionNumber, string content, string changeSummary)
    {
        var version = new PolicyVersion(
            Guid.NewGuid(),
            Id,
            versionNumber,
            content,
            changeSummary);
        
        Versions.Add(version);
        return version;
    }
    
    public void Activate()
    {
        IsActive = true;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
    
    public void SetExpiryDate(DateTime? expiryDate)
    {
        ExpiryDate = expiryDate;
    }
}

