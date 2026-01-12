using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Grc.Assessments;

/// <summary>
/// History/audit log for control assessment changes.
/// Implements IMultiTenant for proper tenant isolation in shared database.
/// </summary>
public class ControlAssessmentHistory : Entity<Guid>, IMultiTenant
{
    /// <summary>
    /// Tenant ID for multi-tenancy support - ABP auto-filters queries by this property
    /// </summary>
    public Guid? TenantId { get; set; }
    
    public Guid ControlAssessmentId { get; private set; }
    public string Action { get; private set; }
    public string Details { get; private set; }
    public Guid? UserId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Dictionary<string, object> OldValues { get; private set; }
    public Dictionary<string, object> NewValues { get; private set; }
    
    protected ControlAssessmentHistory() { }
    
    public ControlAssessmentHistory(Guid controlAssessmentId, string action, string details, Guid? userId = null, Guid? tenantId = null)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ControlAssessmentId = controlAssessmentId;
        Action = action;
        Details = details;
        UserId = userId;
        Timestamp = DateTime.UtcNow;
        OldValues = new Dictionary<string, object>();
        NewValues = new Dictionary<string, object>();
    }
}

