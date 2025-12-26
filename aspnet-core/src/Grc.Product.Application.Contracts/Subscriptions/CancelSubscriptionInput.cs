using System.ComponentModel.DataAnnotations;

namespace Grc.Product.Application.Contracts.Subscriptions;

/// <summary>
/// Input for cancelling a subscription
/// </summary>
public class CancelSubscriptionInput
{
    [StringLength(2000)]
    public string Reason { get; set; }
}
