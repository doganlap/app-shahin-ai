using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when a control is verified
/// </summary>
[EventName("Grc.ControlAssessment.Verified")]
public class ControlVerifiedEto
{
    public Guid ControlAssessmentId { get; set; }
    public Guid VerifierId { get; set; }
    public decimal Score { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime VerificationTime { get; set; }
}

