using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when subscription is activated
/// </summary>
[EventName("Grc.Subscription.Activated")]
public class SubscriptionActivatedEto
{
    public Guid SubscriptionId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime ActivationTime { get; set; }
}

