using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Grc.Assessments;

/// <summary>
/// Junction entity linking assessments to frameworks.
/// Implements IMultiTenant for proper tenant isolation in shared database.
/// </summary>
public class AssessmentFramework : Entity<Guid>, IMultiTenant
{
    /// <summary>
    /// Tenant ID for multi-tenancy support - ABP auto-filters queries by this property
    /// </summary>
    public Guid? TenantId { get; set; }
    
    public Guid AssessmentId { get; private set; }
    public Guid FrameworkId { get; private set; }
    public bool IsMandatory { get; private set; }
    public DateTime CreationTime { get; private set; }
    
    protected AssessmentFramework() { }
    
    public AssessmentFramework(Guid id, Guid assessmentId, Guid frameworkId, bool isMandatory = true, Guid? tenantId = null)
    {
        Id = id;
        TenantId = tenantId;
        AssessmentId = assessmentId;
        FrameworkId = frameworkId;
        IsMandatory = isMandatory;
        CreationTime = DateTime.UtcNow;
    }
}

