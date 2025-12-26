using System;
using System.Collections.Generic;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Grc.Domain.Shared;

namespace Grc.Product.Products;

/// <summary>
/// Subscription product catalog entry
/// </summary>
public class Product : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; private set; }
    public LocalizedString Name { get; private set; }
    public LocalizedString Description { get; private set; }
    public Enums.ProductCategory Category { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public string IconUrl { get; private set; }
    public JsonDocument Metadata { get; private set; }
    
    public ICollection<ProductFeature> Features { get; private set; }
    public ICollection<ProductQuota> Quotas { get; private set; }
    public ICollection<PricingPlan> PricingPlans { get; private set; }
    
    protected Product() { }
    
    public Product(Guid id, string code, LocalizedString name, Enums.ProductCategory category)
        : base(id)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 50);
        Name = Check.NotNull(name, nameof(name));
        Category = category;
        IsActive = true;
        DisplayOrder = 0;
        Features = new List<ProductFeature>();
        Quotas = new List<ProductQuota>();
        PricingPlans = new List<PricingPlan>();
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SetDisplayOrder(int order) => DisplayOrder = order;
    
    public void UpdateMetadata(JsonDocument metadata) => Metadata = metadata;
    public void SetIconUrl(string iconUrl) => IconUrl = iconUrl;
    public void UpdateDescription(LocalizedString description) => Description = description;
}


