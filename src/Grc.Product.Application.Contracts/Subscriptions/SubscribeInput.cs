using System;
using System.ComponentModel.DataAnnotations;

namespace Grc.Product.Subscriptions;

public class SubscribeInput
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    public Guid PricingPlanId { get; set; }
}


