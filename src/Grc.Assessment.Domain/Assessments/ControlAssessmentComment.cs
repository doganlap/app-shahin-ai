using System;
using Volo.Abp.Domain.Entities;

namespace Grc.Assessments;

/// <summary>
/// Comment on a control assessment
/// </summary>
public class ControlAssessmentComment : Entity<Guid>
{
    public Guid ControlAssessmentId { get; private set; }
    public Guid UserId { get; private set; }
    public string Comment { get; private set; }
    public bool IsInternal { get; private set; }
    public DateTime CreationTime { get; private set; }
    
    protected ControlAssessmentComment() { }
    
    public ControlAssessmentComment(Guid id, Guid controlAssessmentId, Guid userId, string comment, bool isInternal = false)
    {
        Id = id;
        ControlAssessmentId = controlAssessmentId;
        UserId = userId;
        Comment = comment;
        IsInternal = isInternal;
        CreationTime = DateTime.UtcNow;
    }
}

