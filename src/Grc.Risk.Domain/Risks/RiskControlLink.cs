using System;
using Volo.Abp.Domain.Entities;

namespace Grc.Risk.Domain.Risks;

/// <summary>
/// Link between risk and control
/// </summary>
public class RiskControlLink : Entity<Guid>
{
    public Guid RiskId { get; private set; }
    public Guid ControlId { get; private set; }
    public string RelationshipType { get; private set; } // Mitigates, Monitors, Detects
    public string Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    protected RiskControlLink() { }
    
    public RiskControlLink(Guid id, Guid riskId, Guid controlId, string relationshipType)
    {
        Id = id;
        RiskId = riskId;
        ControlId = controlId;
        RelationshipType = relationshipType;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void SetNotes(string notes)
    {
        Notes = notes;
    }
}

