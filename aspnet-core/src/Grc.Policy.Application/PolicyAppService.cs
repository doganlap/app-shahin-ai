using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Policy.Domain.Policies;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.Policy.Application;

/// <summary>
/// Application service for Policy operations
/// </summary>
[Authorize(GrcPermissions.Admin.Default)]
public class PolicyAppService : ApplicationService
{
    private readonly IRepository<Policy, Guid> _policyRepository;
    private readonly IRepository<PolicyAttestation, Guid> _attestationRepository;

    public PolicyAppService(
        IRepository<Policy, Guid> policyRepository,
        IRepository<PolicyAttestation, Guid> attestationRepository)
    {
        _policyRepository = policyRepository;
        _attestationRepository = attestationRepository;
    }

    public async Task<Policy> CreatePolicyAsync(
        string policyCode,
        ValueObjects.LocalizedString title,
        Guid ownerUserId,
        DateTime effectiveDate)
    {
        var policy = new Policy(
            GuidGenerator.Create(),
            policyCode,
            title,
            ownerUserId,
            effectiveDate);
        
        await _policyRepository.InsertAsync(policy);
        return policy;
    }

    public async Task<PolicyVersion> CreateVersionAsync(
        Guid policyId,
        string versionNumber,
        string content,
        string changeSummary)
    {
        var policy = await _policyRepository.GetAsync(policyId);
        
        // Mark previous versions as not current
        foreach (var version in policy.Versions)
        {
            version.IsCurrentVersion = false;
        }
        
        var version = policy.CreateVersion(versionNumber, content, changeSummary);
        version.MarkAsCurrent();
        
        await _policyRepository.UpdateAsync(policy);
        return version;
    }

    public async Task AttestPolicyAsync(Guid policyId, Guid policyVersionId, string ipAddress)
    {
        var currentUserId = CurrentUser.Id ?? throw new UnauthorizedAccessException();
        
        var attestation = new PolicyAttestation(
            GuidGenerator.Create(),
            policyId,
            policyVersionId,
            currentUserId.Value,
            ipAddress);
        
        await _attestationRepository.InsertAsync(attestation);
    }

    public async Task<List<Policy>> GetPoliciesRequiringAttestationAsync(Guid userId)
    {
        var allPolicies = await _policyRepository.GetListAsync(p => p.IsActive);
        var attestedPolicies = await _attestationRepository.GetListAsync(a => a.UserId == userId);
        var attestedPolicyIds = new HashSet<Guid>(attestedPolicies.Select(a => a.PolicyId));
        
        return allPolicies.Where(p => !attestedPolicyIds.Contains(p.Id)).ToList();
    }
}

