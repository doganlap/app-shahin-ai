using System;
using System.ComponentModel.DataAnnotations;

namespace Grc.Product.Application.Contracts.Subscriptions;

/// <summary>
/// Input for upgrading a subscription
/// </summary>
public class UpgradeSubscriptionInput
{
    [Required]
    public Guid NewProductId { get; set; }
    
    public Guid? NewPricingPlanId { get; set; }
    
    [Required]
    public DateTime EffectiveDate { get; set; }
}
