using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Enums;
using Grc.Product.Application.Contracts.Subscriptions;
using Grc.Product.Products;
using Grc.Product.Services;
using Grc.Product.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.Product.Application.Subscriptions;

/// <summary>
/// Application service for Subscription operations
/// </summary>
[Authorize]
public class SubscriptionAppService : ApplicationService, ISubscriptionAppService
{
    private readonly ITenantSubscriptionRepository _subscriptionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IQuotaUsageRepository _quotaUsageRepository;
    private readonly QuotaEnforcementService _quotaEnforcementService;

    public SubscriptionAppService(
        ITenantSubscriptionRepository subscriptionRepository,
        IProductRepository productRepository,
        IQuotaUsageRepository quotaUsageRepository,
        QuotaEnforcementService quotaEnforcementService)
    {
        _subscriptionRepository = subscriptionRepository;
        _productRepository = productRepository;
        _quotaUsageRepository = quotaUsageRepository;
        _quotaEnforcementService = quotaEnforcementService;
    }

    public async Task<TenantSubscriptionDto> SubscribeAsync(SubscribeInput input)
    {
        var tenantId = CurrentTenant.Id ?? throw new UnauthorizedAccessException("Tenant ID required");
        
        var product = await _productRepository.GetAsync(input.ProductId);
        if (!product.IsActive)
        {
            throw new InvalidOperationException("Product is not active");
        }

        var subscription = new TenantSubscription(
            GuidGenerator.Create(),
            tenantId.Value,
            input.ProductId,
            input.StartDate,
            input.PricingPlanId)
        {
            TenantId = tenantId
        };

        await _subscriptionRepository.InsertAsync(subscription);
        return ObjectMapper.Map<TenantSubscription, TenantSubscriptionDto>(subscription);
    }

    public async Task<SubscriptionDetailDto> GetCurrentSubscriptionAsync()
    {
        var tenantId = CurrentTenant.Id ?? throw new UnauthorizedAccessException("Tenant ID required");
        var subscription = await _subscriptionRepository.GetActiveSubscriptionAsync(tenantId.Value);
        
        if (subscription == null)
        {
            throw new InvalidOperationException("No active subscription found");
        }

        return await GetSubscriptionDetailAsync(subscription.Id);
    }

    public async Task<SubscriptionDetailDto> GetAsync(Guid id)
    {
        return await GetSubscriptionDetailAsync(id);
    }

    public async Task<TenantSubscriptionDto> CancelAsync(Guid id, CancelSubscriptionInput input)
    {
        var subscription = await _subscriptionRepository.GetAsync(id);
        subscription.Cancel(input.Reason ?? "Cancelled by user");
        
        await _subscriptionRepository.UpdateAsync(subscription);
        return ObjectMapper.Map<TenantSubscription, TenantSubscriptionDto>(subscription);
    }

    public async Task<TenantSubscriptionDto> UpgradeAsync(Guid id, UpgradeSubscriptionInput input)
    {
        var subscription = await _subscriptionRepository.GetAsync(id);
        
        // Create new subscription for upgraded product
        var newSubscription = new TenantSubscription(
            GuidGenerator.Create(),
            subscription.TenantId!.Value,
            input.NewProductId,
            input.EffectiveDate,
            input.NewPricingPlanId)
        {
            TenantId = subscription.TenantId
        };
        
        // Cancel old subscription
        subscription.Cancel("Upgraded to new product");
        
        await _subscriptionRepository.InsertAsync(newSubscription);
        await _subscriptionRepository.UpdateAsync(subscription);
        
        return ObjectMapper.Map<TenantSubscription, TenantSubscriptionDto>(newSubscription);
    }

    public async Task<QuotaCheckResult> CheckQuotaAsync(QuotaCheckInput input)
    {
        var tenantId = CurrentTenant.Id ?? throw new UnauthorizedAccessException("Tenant ID required");
        var usageInfo = await _quotaEnforcementService.GetQuotaUsageAsync(tenantId.Value, input.QuotaType);
        
        if (!usageInfo.HasSubscription)
        {
            return new QuotaCheckResult
            {
                Allowed = false,
                CurrentUsage = 0,
                Limit = null,
                Remaining = 0
            };
        }

        var allowed = await _quotaEnforcementService.CheckQuotaAsync(tenantId.Value, input.QuotaType, input.RequestedAmount);
        
        return new QuotaCheckResult
        {
            Allowed = allowed,
            CurrentUsage = usageInfo.CurrentUsage,
            Limit = usageInfo.Limit,
            Remaining = usageInfo.Remaining ?? 0
        };
    }

    private async Task<SubscriptionDetailDto> GetSubscriptionDetailAsync(Guid subscriptionId)
    {
        var subscription = await _subscriptionRepository.GetAsync(subscriptionId);
        var product = await _productRepository.GetAsync(subscription.ProductId);
        
        var dto = ObjectMapper.Map<TenantSubscription, SubscriptionDetailDto>(subscription);
        dto.Product = ObjectMapper.Map<Product, ProductDto>(product);
        
        // Load quota usages
        var quotaUsages = await _quotaUsageRepository.GetListAsync(u => u.TenantId == subscription.TenantId);
        dto.QuotaUsages = ObjectMapper.Map<List<QuotaUsage>, List<QuotaUsageDto>>(quotaUsages);
        
        // Calculate quota statuses
        dto.QuotaStatuses = new Dictionary<QuotaType, QuotaStatusDto>();
        foreach (var quota in product.Quotas)
        {
            var usage = quotaUsages.FirstOrDefault(u => u.QuotaType == quota.QuotaType);
            var status = new QuotaStatusDto
            {
                CurrentUsage = usage?.CurrentUsage ?? 0,
                Limit = quota.Limit,
                IsUnlimited = quota.IsUnlimited,
                PercentageUsed = quota.Limit.HasValue ? (usage?.CurrentUsage ?? 0) / quota.Limit.Value * 100 : 0,
                IsExceeded = quota.Limit.HasValue && (usage?.CurrentUsage ?? 0) > quota.Limit.Value
            };
            dto.QuotaStatuses[quota.QuotaType] = status;
        }
        
        return dto;
    }
}
