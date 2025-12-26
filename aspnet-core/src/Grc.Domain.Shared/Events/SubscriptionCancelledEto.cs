using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when subscription is cancelled
/// </summary>
[EventName("Grc.Subscription.Cancelled")]
public class SubscriptionCancelledEto
{
    public Guid SubscriptionId { get; set; }
    public Guid? TenantId { get; set; }
    public string Reason { get; set; }
    public DateTime CancellationTime { get; set; }
}

