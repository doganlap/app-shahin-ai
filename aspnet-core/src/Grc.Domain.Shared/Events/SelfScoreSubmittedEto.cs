using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when a self-score is submitted
/// </summary>
[EventName("Grc.ControlAssessment.SelfScoreSubmitted")]
public class SelfScoreSubmittedEto
{
    public Guid ControlAssessmentId { get; set; }
    public decimal Score { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime SubmissionTime { get; set; }
}

