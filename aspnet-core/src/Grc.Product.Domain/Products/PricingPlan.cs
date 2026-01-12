using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Grc.Product.Products;

/// <summary>
/// Pricing plan for a product
/// </summary>
public class PricingPlan : FullAuditedEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public Enums.BillingPeriod BillingPeriod { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; }
    public int? TrialDays { get; private set; }
    public bool IsActive { get; private set; }
    public string StripePriceId { get; private set; }
    
    protected PricingPlan() { }
    
    public PricingPlan(Guid id, Guid productId, Enums.BillingPeriod billingPeriod, decimal price)
        : base(id)
    {
        ProductId = productId;
        BillingPeriod = billingPeriod;
        Price = price;
        Currency = "SAR";
        IsActive = true;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SetStripePriceId(string stripePriceId) => StripePriceId = stripePriceId;
    public void UpdatePrice(decimal price) => Price = price;
    public void SetTrialDays(int? trialDays) => TrialDays = trialDays;
}


