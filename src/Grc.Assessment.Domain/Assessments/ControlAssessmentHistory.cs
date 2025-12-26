using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Grc.Assessments;

/// <summary>
/// History/audit log for control assessment changes
/// </summary>
public class ControlAssessmentHistory : Entity<Guid>
{
    public Guid ControlAssessmentId { get; private set; }
    public string Action { get; private set; }
    public string Details { get; private set; }
    public Guid? UserId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Dictionary<string, object> OldValues { get; private set; }
    public Dictionary<string, object> NewValues { get; private set; }
    
    protected ControlAssessmentHistory() { }
    
    public ControlAssessmentHistory(Guid controlAssessmentId, string action, string details, Guid? userId = null)
    {
        Id = Guid.NewGuid();
        ControlAssessmentId = controlAssessmentId;
        Action = action;
        Details = details;
        UserId = userId;
        Timestamp = DateTime.UtcNow;
        OldValues = new Dictionary<string, object>();
        NewValues = new Dictionary<string, object>();
    }
}

