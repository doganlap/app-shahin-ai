using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Grc.Product.Products;
using Grc.Product.Services;
using Grc.Product.Subscriptions;

namespace Grc.Product.Subscriptions;

[Authorize]
public class SubscriptionAppService : ApplicationService, ISubscriptionAppService
{
    private readonly ITenantSubscriptionRepository _subscriptionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IQuotaUsageRepository _quotaUsageRepository;
    private readonly QuotaEnforcementService _quotaEnforcementService;
    private readonly ICurrentTenant _currentTenant;

    public SubscriptionAppService(
        ITenantSubscriptionRepository subscriptionRepository,
        IProductRepository productRepository,
        IQuotaUsageRepository quotaUsageRepository,
        QuotaEnforcementService quotaEnforcementService,
        ICurrentTenant currentTenant)
    {
        _subscriptionRepository = subscriptionRepository;
        _productRepository = productRepository;
        _quotaUsageRepository = quotaUsageRepository;
        _quotaEnforcementService = quotaEnforcementService;
        _currentTenant = currentTenant;
    }

    public async Task<SubscriptionDetailDto> GetCurrentSubscriptionAsync()
    {
        var tenantId = _currentTenant.Id;
        if (!tenantId.HasValue)
            throw new BusinessException("Tenant context required");

        var subscription = await _subscriptionRepository.GetCurrentSubscriptionAsync(tenantId.Value);
        if (subscription == null)
            throw new EntityNotFoundException(typeof(TenantSubscription));

        return ObjectMapper.Map<TenantSubscription, SubscriptionDetailDto>(subscription);
    }

    public async Task<TenantSubscriptionDto> SubscribeAsync(SubscribeInput input)
    {
        var tenantId = _currentTenant.Id;
        if (!tenantId.HasValue)
            throw new BusinessException("Tenant context required");

        // Check if tenant already has an active subscription
        var existing = await _subscriptionRepository.GetActiveSubscriptionAsync(tenantId.Value);
        if (existing != null)
            throw new BusinessException("Tenant already has an active subscription. Please cancel or upgrade existing subscription.");

        var product = await _productRepository.GetAsync(input.ProductId);
        if (product == null)
            throw new EntityNotFoundException(typeof(Product), input.ProductId);

        var pricingPlan = product.PricingPlans?.FirstOrDefault(p => p.Id == input.PricingPlanId && p.IsActive);
        if (pricingPlan == null)
            throw new BusinessException("Invalid or inactive pricing plan");

        var startDate = DateTime.UtcNow.Date;
        var endDate = pricingPlan.BillingPeriod switch
        {
            Enums.BillingPeriod.Monthly => startDate.AddMonths(1),
            Enums.BillingPeriod.Quarterly => startDate.AddMonths(3),
            Enums.BillingPeriod.Annual => startDate.AddYears(1),
            _ => (DateTime?)null
        };

        var trialEndDate = pricingPlan.TrialDays.HasValue
            ? startDate.AddDays(pricingPlan.TrialDays.Value)
            : (DateTime?)null;

        var subscription = new TenantSubscription(
            GuidGenerator.Create(),
            tenantId.Value,
            input.ProductId,
            startDate,
            input.PricingPlanId
        );

        subscription.SetEndDate(endDate);
        subscription.SetTrialEndDate(trialEndDate);

        await _subscriptionRepository.InsertAsync(subscription);

        return ObjectMapper.Map<TenantSubscription, TenantSubscriptionDto>(subscription);
    }

    public async Task<TenantSubscriptionDto> CancelAsync(Guid subscriptionId, CancelSubscriptionInput input)
    {
        var tenantId = _currentTenant.Id;
        if (!tenantId.HasValue)
            throw new BusinessException("Tenant context required");

        var subscription = await _subscriptionRepository.GetAsync(subscriptionId);
        if (subscription.TenantId != tenantId.Value)
            throw new BusinessException("Subscription not found for current tenant");

        subscription.Cancel(input.Reason);
        await _subscriptionRepository.UpdateAsync(subscription);

        return ObjectMapper.Map<TenantSubscription, TenantSubscriptionDto>(subscription);
    }

    public async Task<TenantSubscriptionDto> UpgradeAsync(UpgradeSubscriptionInput input)
    {
        var tenantId = _currentTenant.Id;
        if (!tenantId.HasValue)
            throw new BusinessException("Tenant context required");

        var currentSubscription = await _subscriptionRepository.GetActiveSubscriptionAsync(tenantId.Value);
        if (currentSubscription == null)
            throw new BusinessException("No active subscription found");

        // Cancel current subscription
        currentSubscription.Cancel("Upgraded to new plan");

        // Create new subscription
        var subscribeInput = new SubscribeInput
        {
            ProductId = input.ProductId,
            PricingPlanId = input.PricingPlanId
        };

        return await SubscribeAsync(subscribeInput);
    }

    public async Task<List<QuotaUsageDto>> GetQuotaUsageAsync(string quotaType = null)
    {
        var tenantId = _currentTenant.Id;
        if (!tenantId.HasValue)
            throw new BusinessException("Tenant context required");

        var quotaTypes = string.IsNullOrEmpty(quotaType)
            ? Enum.GetValues<Enums.QuotaType>()
            : new[] { Enum.Parse<Enums.QuotaType>(quotaType) };

        var result = new List<QuotaUsageDto>();

        foreach (var type in quotaTypes)
        {
            var usageInfo = await _quotaEnforcementService.GetQuotaUsageAsync(tenantId.Value, type);
            
            result.Add(new QuotaUsageDto
            {
                QuotaType = type.ToString(),
                CurrentUsage = usageInfo.CurrentUsage,
                Limit = usageInfo.Limit,
                Unit = usageInfo.Unit,
                Remaining = usageInfo.Remaining,
                ResetDate = usageInfo.ResetDate,
                PercentageUsed = usageInfo.PercentageUsed,
                IsUnlimited = usageInfo.IsUnlimited
            });
        }

        return result;
    }

    public async Task<QuotaCheckResult> CheckQuotaAsync(string quotaType, decimal requestedAmount)
    {
        var tenantId = _currentTenant.Id;
        if (!tenantId.HasValue)
            throw new BusinessException("Tenant context required");

        var type = Enum.Parse<Enums.QuotaType>(quotaType);
        var allowed = await _quotaEnforcementService.CheckQuotaAsync(tenantId.Value, type, requestedAmount);
        var usageInfo = await _quotaEnforcementService.GetQuotaUsageAsync(tenantId.Value, type);

        return new QuotaCheckResult
        {
            Allowed = allowed,
            CurrentUsage = usageInfo.CurrentUsage,
            Limit = usageInfo.Limit,
            Remaining = usageInfo.Remaining
        };
    }
}

