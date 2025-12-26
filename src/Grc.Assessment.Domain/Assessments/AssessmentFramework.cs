using System;
using Volo.Abp.Domain.Entities;

namespace Grc.Assessments;

/// <summary>
/// Junction entity linking assessments to frameworks
/// </summary>
public class AssessmentFramework : Entity<Guid>
{
    public Guid AssessmentId { get; private set; }
    public Guid FrameworkId { get; private set; }
    public bool IsMandatory { get; private set; }
    public DateTime CreationTime { get; private set; }
    
    protected AssessmentFramework() { }
    
    public AssessmentFramework(Guid id, Guid assessmentId, Guid frameworkId, bool isMandatory = true)
    {
        Id = id;
        AssessmentId = assessmentId;
        FrameworkId = frameworkId;
        IsMandatory = isMandatory;
        CreationTime = DateTime.UtcNow;
    }
}

