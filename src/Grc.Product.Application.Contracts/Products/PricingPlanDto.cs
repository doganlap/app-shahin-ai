using System;
using Grc.Enums;

namespace Grc.Product.Application.Contracts.Products;

/// <summary>
/// Pricing plan DTO
/// </summary>
public class PricingPlanDto
{
    public Guid Id { get; set; }
    public BillingPeriod BillingPeriod { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public int? TrialDays { get; set; }
    public bool IsActive { get; set; }
    public string StripePriceId { get; set; }
}
