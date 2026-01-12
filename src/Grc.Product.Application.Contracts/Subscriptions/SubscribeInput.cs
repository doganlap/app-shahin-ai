using System;
using System.ComponentModel.DataAnnotations;

namespace Grc.Product.Application.Contracts.Subscriptions;

/// <summary>
/// Input for subscribing to a product
/// </summary>
public class SubscribeInput
{
    [Required]
    public Guid ProductId { get; set; }
    
    public Guid? PricingPlanId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
}
