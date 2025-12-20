using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Grc.Product.Subscriptions;

public interface ISubscriptionAppService : IApplicationService
{
    Task<SubscriptionDetailDto> GetCurrentSubscriptionAsync();
    Task<TenantSubscriptionDto> SubscribeAsync(SubscribeInput input);
    Task<TenantSubscriptionDto> CancelAsync(Guid subscriptionId, CancelSubscriptionInput input);
    Task<TenantSubscriptionDto> UpgradeAsync(UpgradeSubscriptionInput input);
    Task<List<QuotaUsageDto>> GetQuotaUsageAsync(string quotaType = null);
    Task<QuotaCheckResult> CheckQuotaAsync(string quotaType, decimal requestedAmount);
}

public class QuotaCheckResult
{
    public bool Allowed { get; set; }
    public decimal CurrentUsage { get; set; }
    public decimal? Limit { get; set; }
    public decimal? Remaining { get; set; }
}


