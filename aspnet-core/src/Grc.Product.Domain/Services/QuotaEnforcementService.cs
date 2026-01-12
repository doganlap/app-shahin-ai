using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Grc.Product.Products;
using Grc.Product.Subscriptions;

namespace Grc.Product.Services;

/// <summary>
/// Domain service for quota validation and enforcement
/// </summary>
public class QuotaEnforcementService : DomainService
{
    private readonly IProductRepository _productRepository;
    private readonly ITenantSubscriptionRepository _subscriptionRepository;
    private readonly IQuotaUsageRepository _quotaUsageRepository;
    
    public QuotaEnforcementService(
        IProductRepository productRepository,
        ITenantSubscriptionRepository subscriptionRepository,
        IQuotaUsageRepository quotaUsageRepository)
    {
        _productRepository = productRepository;
        _subscriptionRepository = subscriptionRepository;
        _quotaUsageRepository = quotaUsageRepository;
    }
    
    /// <summary>
    /// Check if quota allows the requested operation
    /// </summary>
    public async Task<bool> CheckQuotaAsync(Guid tenantId, Enums.QuotaType quotaType, decimal requestedAmount)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionAsync(tenantId);
        if (subscription == null)
            return false; // No active subscription
        
        var product = await _productRepository.GetAsync(subscription.ProductId);
        if (product == null)
            return false;
        
        var quota = product.Quotas?.FirstOrDefault(q => q.QuotaType == quotaType && q.IsEnforced);
        if (quota == null || !quota.IsEnforced)
            return true; // No quota enforcement for this type
        
        if (quota.IsUnlimited)
            return true;
        
        var usage = await _quotaUsageRepository.GetOrCreateAsync(tenantId, quotaType);
        var newUsage = usage.CurrentUsage + requestedAmount;
        
        return newUsage <= quota.Limit.Value;
    }
    
    /// <summary>
    /// Reserve quota for an operation
    /// </summary>
    public async Task ReserveQuotaAsync(Guid tenantId, Enums.QuotaType quotaType, decimal amount)
    {
        var allowed = await CheckQuotaAsync(tenantId, quotaType, amount);
        if (!allowed)
            throw new BusinessException($"Quota limit exceeded for {quotaType}. Cannot reserve {amount}.");
        
        var usage = await _quotaUsageRepository.GetOrCreateAsync(tenantId, quotaType);
        usage.Increment(amount);
        // Note: Persistence should be handled by the calling application service
    }
    
    /// <summary>
    /// Release reserved quota
    /// </summary>
    public async Task ReleaseQuotaAsync(Guid tenantId, Enums.QuotaType quotaType, decimal amount)
    {
        var usage = await _quotaUsageRepository.GetOrCreateAsync(tenantId, quotaType);
        usage.Decrement(amount);
        // Note: Persistence should be handled by the calling application service
    }
    
    /// <summary>
    /// Get current quota usage
    /// </summary>
    public async Task<QuotaUsageInfo> GetQuotaUsageAsync(Guid tenantId, Enums.QuotaType quotaType)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionAsync(tenantId);
        if (subscription == null)
            return new QuotaUsageInfo { HasSubscription = false };
        
        var product = await _productRepository.GetAsync(subscription.ProductId);
        var quota = product?.Quotas?.FirstOrDefault(q => q.QuotaType == quotaType);
        var usage = await _quotaUsageRepository.GetOrCreateAsync(tenantId, quotaType);
        
        return new QuotaUsageInfo
        {
            HasSubscription = true,
            CurrentUsage = usage.CurrentUsage,
            Limit = quota?.Limit,
            IsUnlimited = quota?.IsUnlimited ?? false,
            IsEnforced = quota?.IsEnforced ?? false,
            Unit = quota?.Unit,
            ResetDate = usage.ResetDate
        };
    }
}

/// <summary>
/// Information about quota usage
/// </summary>
public class QuotaUsageInfo
{
    public bool HasSubscription { get; set; }
    public decimal CurrentUsage { get; set; }
    public decimal? Limit { get; set; }
    public bool IsUnlimited { get; set; }
    public bool IsEnforced { get; set; }
    public string Unit { get; set; }
    public DateTime? ResetDate { get; set; }
    
    public decimal? Remaining => Limit.HasValue ? Limit.Value - CurrentUsage : null;
    public decimal? PercentageUsed => Limit.HasValue ? (CurrentUsage / Limit.Value) * 100 : null;
}

