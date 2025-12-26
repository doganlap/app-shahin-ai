using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when an assessment is completed
/// </summary>
[EventName("Grc.Assessment.Completed")]
public class AssessmentCompletedEto
{
    public Guid AssessmentId { get; set; }
    public Guid? TenantId { get; set; }
    public decimal OverallScore { get; set; }
    public DateTime CompletionTime { get; set; }
}

