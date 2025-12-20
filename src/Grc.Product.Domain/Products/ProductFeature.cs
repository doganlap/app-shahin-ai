using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Grc.Domain.Shared;

namespace Grc.Product.Products;

/// <summary>
/// Feature included in a product
/// </summary>
public class ProductFeature : FullAuditedEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public string FeatureCode { get; private set; }
    public LocalizedString Name { get; private set; }
    public LocalizedString Description { get; private set; }
    public Enums.FeatureType FeatureType { get; private set; }
    public string Value { get; private set; }
    public bool IsEnabled { get; private set; }
    
    protected ProductFeature() { }
    
    public ProductFeature(Guid id, Guid productId, string featureCode, LocalizedString name, Enums.FeatureType featureType)
        : base(id)
    {
        ProductId = productId;
        FeatureCode = Check.NotNullOrWhiteSpace(featureCode, nameof(featureCode));
        Name = Check.NotNull(name, nameof(name));
        FeatureType = featureType;
        IsEnabled = true;
    }
    
    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
    public void SetValue(string value) => Value = value;
    public void UpdateDescription(LocalizedString description) => Description = description;
}


