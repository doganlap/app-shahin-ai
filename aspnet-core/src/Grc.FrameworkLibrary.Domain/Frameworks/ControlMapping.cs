using System;
using Volo.Abp.Domain.Entities;

namespace Grc.FrameworkLibrary.Domain.Frameworks;

/// <summary>
/// Cross-framework control mapping
/// </summary>
public class ControlMapping : Entity<Guid>
{
    public Guid SourceControlId { get; private set; }
    public string TargetFrameworkCode { get; private set; }
    public string TargetControlNumber { get; private set; }
    public string MappingStrength { get; private set; } // Exact, Strong, Partial, Weak
    public string Notes { get; private set; }
    public DateTime CreationTime { get; private set; }
    
    protected ControlMapping() { }
    
    public ControlMapping(Guid sourceControlId, string targetFrameworkCode, string targetControlNumber, string mappingStrength)
    {
        Id = Guid.NewGuid();
        SourceControlId = sourceControlId;
        TargetFrameworkCode = targetFrameworkCode;
        TargetControlNumber = targetControlNumber;
        MappingStrength = mappingStrength;
        CreationTime = DateTime.UtcNow;
    }
    
    public void SetNotes(string notes)
    {
        Notes = notes;
    }
}

