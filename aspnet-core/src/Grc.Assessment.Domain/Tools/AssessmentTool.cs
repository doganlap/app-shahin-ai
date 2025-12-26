using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Assessment.Domain.Tools;

/// <summary>
/// Assessment tool entity for tracking tools used in assessments
/// </summary>
public class AssessmentTool : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ToolCategory Category { get; private set; }
    public string? Vendor { get; private set; }
    public string? Version { get; private set; }
    public string? Website { get; private set; }
    public bool IsActive { get; private set; }
    
    protected AssessmentTool() { }
    
    public AssessmentTool(Guid id, string name, ToolCategory category, string? vendor = null)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 200);
        Category = category;
        Vendor = vendor;
        IsActive = true;
    }
    
    public void SetDescription(string? description)
    {
        Description = description;
    }
    
    public void SetVersion(string? version)
    {
        Version = version;
    }
    
    public void SetWebsite(string? website)
    {
        Website = website;
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
