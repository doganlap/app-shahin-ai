using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when an assessment is started
/// </summary>
[EventName("Grc.Assessment.Started")]
public class AssessmentStartedEto
{
    public Guid AssessmentId { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime StartTime { get; set; }
}

