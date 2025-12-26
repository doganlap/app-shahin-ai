using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when a control is assigned to a user
/// </summary>
[EventName("Grc.ControlAssessment.Assigned")]
public class ControlAssignedEto
{
    public Guid ControlAssessmentId { get; set; }
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime AssignmentTime { get; set; }
}

