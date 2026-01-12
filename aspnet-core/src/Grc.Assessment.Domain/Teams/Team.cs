using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Assessment.Domain.Teams;

/// <summary>
/// Assessment team for managing group-based assessments
/// </summary>
public class Team : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TeamType Type { get; private set; }
    public bool IsActive { get; private set; }
    
    public ICollection<TeamMember> Members { get; private set; } = new Collection<TeamMember>();
    
    protected Team() { }
    
    public Team(Guid id, string name, TeamType type, string? description = null)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 200);
        Type = type;
        Description = description;
        IsActive = true;
        Members = new Collection<TeamMember>();
    }
    
    public TeamMember AddMember(Guid userId, string? role, bool isLead = false)
    {
        var member = new TeamMember(Guid.NewGuid(), Id, userId, role, isLead)
        {
            TenantId = TenantId
        };
        Members.Add(member);
        return member;
    }
    
    public void SetDescription(string description)
    {
        Description = description;
    }
    
    public void Activate()
    {
        IsActive = true;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
}
