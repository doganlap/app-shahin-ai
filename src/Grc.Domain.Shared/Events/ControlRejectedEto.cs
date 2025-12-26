using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when a control assessment is rejected
/// </summary>
[EventName("Grc.ControlAssessment.Rejected")]
public class ControlRejectedEto
{
    public Guid ControlAssessmentId { get; set; }
    public Guid VerifierId { get; set; }
    public string Reason { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime RejectionTime { get; set; }
}

