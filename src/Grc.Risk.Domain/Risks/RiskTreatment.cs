using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Grc.Risk.Domain.Risks;

/// <summary>
/// Risk treatment plan
/// </summary>
public class RiskTreatment : Entity<Guid>
{
    public Guid RiskId { get; private set; }
    public string TreatmentType { get; private set; } // Mitigate, Transfer, Accept, Avoid
    public string Description { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public DateTime? TargetDate { get; private set; }
    public string Status { get; private set; }
    public Dictionary<string, object> TreatmentData { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    
    protected RiskTreatment() { }
    
    public RiskTreatment(Guid id, Guid riskId, string treatmentType, string description)
    {
        Id = id;
        RiskId = riskId;
        TreatmentType = treatmentType;
        Description = description;
        Status = "Planned";
        TreatmentData = new Dictionary<string, object>();
        CreatedAt = DateTime.UtcNow;
    }
    
    public void AssignTo(Guid userId, DateTime? targetDate = null)
    {
        AssignedToUserId = userId;
        TargetDate = targetDate;
    }
    
    public void Complete()
    {
        Status = "Completed";
        CompletedAt = DateTime.UtcNow;
    }
}

