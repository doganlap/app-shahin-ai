using System;
using Volo.Abp.Domain.Entities;

namespace Grc.Policy.Domain.Policies;

/// <summary>
/// Policy attestation (acknowledgment)
/// </summary>
public class PolicyAttestation : Entity<Guid>
{
    public Guid PolicyId { get; private set; }
    public Guid PolicyVersionId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime AttestedAt { get; private set; }
    public string IpAddress { get; private set; }
    public bool IsAcknowledged { get; private set; }
    
    protected PolicyAttestation() { }
    
    public PolicyAttestation(Guid id, Guid policyId, Guid policyVersionId, Guid userId, string ipAddress)
    {
        Id = id;
        PolicyId = policyId;
        PolicyVersionId = policyVersionId;
        UserId = userId;
        IpAddress = ipAddress;
        AttestedAt = DateTime.UtcNow;
        IsAcknowledged = true;
    }
}

