using System;

namespace Grc.Product.Subscriptions;

public class SubscriptionDetailDto : TenantSubscriptionDto
{
    public DateTime? CancelledAt { get; set; }
    public string CancellationReason { get; set; }
}


