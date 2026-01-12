using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when an assessment is created
/// </summary>
[EventName("Grc.Assessment.Created")]
public class AssessmentCreatedEto
{
    public Guid AssessmentId { get; set; }
    public string Name { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }
}

