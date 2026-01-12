using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Assessment.Domain.Teams;

/// <summary>
/// Team member entity representing a user assigned to an assessment team
/// </summary>
public class TeamMember : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }
    public string Role { get; private set; }
    public bool IsLead { get; private set; }
    public bool IsActive { get; private set; }
    
    protected TeamMember() { }
    
    public TeamMember(Guid id, Guid teamId, Guid userId, string role, bool isLead = false)
    {
        Id = id;
        TeamId = teamId;
        UserId = userId;
        Role = role;
        IsLead = isLead;
        IsActive = true;
    }
    
    public void PromoteToLead()
    {
        IsLead = true;
    }
    
    public void DemoteFromLead()
    {
        IsLead = false;
    }
    
    public void SetRole(string role)
    {
        Role = role;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
    
    public void Activate()
    {
        IsActive = true;
    }
}
