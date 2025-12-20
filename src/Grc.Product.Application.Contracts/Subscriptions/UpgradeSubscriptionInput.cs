using System;
using System.ComponentModel.DataAnnotations;

namespace Grc.Product.Subscriptions;

public class UpgradeSubscriptionInput
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    public Guid PricingPlanId { get; set; }
}


