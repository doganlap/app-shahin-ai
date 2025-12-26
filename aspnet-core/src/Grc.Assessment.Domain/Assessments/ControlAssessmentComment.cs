using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Grc.Assessments;

/// <summary>
/// Comment on a control assessment.
/// Implements IMultiTenant for proper tenant isolation in shared database.
/// </summary>
public class ControlAssessmentComment : Entity<Guid>, IMultiTenant
{
    /// <summary>
    /// Tenant ID for multi-tenancy support - ABP auto-filters queries by this property
    /// </summary>
    public Guid? TenantId { get; set; }
    
    public Guid ControlAssessmentId { get; private set; }
    public Guid UserId { get; private set; }
    public string Comment { get; private set; }
    public bool IsInternal { get; private set; }
    public DateTime CreationTime { get; private set; }
    
    protected ControlAssessmentComment() { }
    
    public ControlAssessmentComment(Guid id, Guid controlAssessmentId, Guid userId, string comment, bool isInternal = false, Guid? tenantId = null)
    {
        Id = id;
        TenantId = tenantId;
        ControlAssessmentId = controlAssessmentId;
        UserId = userId;
        Comment = comment;
        IsInternal = isInternal;
        CreationTime = DateTime.UtcNow;
    }
}

