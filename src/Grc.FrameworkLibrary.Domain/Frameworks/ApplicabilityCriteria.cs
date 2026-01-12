using System;
using Volo.Abp.Domain.Entities;

namespace Grc.FrameworkLibrary.Frameworks;

/// <summary>
/// Applicability criteria for frameworks
/// </summary>
public class ApplicabilityCriteria : Entity<Guid>
{
    public Guid FrameworkId { get; private set; }
    public string CriteriaType { get; private set; } // Sector, EntityType, EmployeeCount, etc.
    public string CriteriaValue { get; private set; }
    public bool IsRequired { get; private set; }
    public DateTime CreationTime { get; private set; }
    
    protected ApplicabilityCriteria() { }
    
    public ApplicabilityCriteria(Guid id, Guid frameworkId, string criteriaType, string criteriaValue, bool isRequired = true)
    {
        Id = id;
        FrameworkId = frameworkId;
        CriteriaType = criteriaType;
        CriteriaValue = criteriaValue;
        IsRequired = isRequired;
        CreationTime = DateTime.UtcNow;
    }
}

