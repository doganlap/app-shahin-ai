using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Grc.Product.Products;

/// <summary>
/// Quota limit for a product
/// </summary>
public class ProductQuota : FullAuditedEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public Enums.QuotaType QuotaType { get; private set; }
    public decimal? Limit { get; private set; }
    public string Unit { get; private set; }
    public bool IsEnforced { get; private set; }
    
    protected ProductQuota() { }
    
    public ProductQuota(Guid id, Guid productId, Enums.QuotaType quotaType, decimal? limit = null)
        : base(id)
    {
        ProductId = productId;
        QuotaType = quotaType;
        Limit = limit; // null means unlimited
        IsEnforced = true;
    }
    
    public bool IsUnlimited => Limit == null;
    
    public bool AllowsAmount(decimal amount)
    {
        if (!IsEnforced) return true;
        if (IsUnlimited) return true;
        return amount <= Limit.Value;
    }
    
    public void UpdateLimit(decimal? limit) => Limit = limit;
    public void SetUnit(string unit) => Unit = unit;
    public void SetEnforced(bool enforced) => IsEnforced = enforced;
}


