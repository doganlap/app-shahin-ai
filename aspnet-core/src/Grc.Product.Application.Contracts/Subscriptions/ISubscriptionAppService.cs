using System;
using System.Threading.Tasks;
using Grc.Enums;
using Volo.Abp.Application.Services;

namespace Grc.Product.Application.Contracts.Subscriptions;

/// <summary>
/// Application service interface for Subscription operations
/// </summary>
public interface ISubscriptionAppService : IApplicationService
{
    Task<TenantSubscriptionDto> SubscribeAsync(SubscribeInput input);
    Task<SubscriptionDetailDto> GetCurrentSubscriptionAsync();
    Task<SubscriptionDetailDto> GetAsync(Guid id);
    Task<TenantSubscriptionDto> CancelAsync(Guid id, CancelSubscriptionInput input);
    Task<TenantSubscriptionDto> UpgradeAsync(Guid id, UpgradeSubscriptionInput input);
    Task<QuotaCheckResult> CheckQuotaAsync(QuotaCheckInput input);
}

/// <summary>
/// Quota check input
/// </summary>
public class QuotaCheckInput
{
    public QuotaType QuotaType { get; set; }
    public decimal RequestedAmount { get; set; }
}

/// <summary>
/// Quota check result
/// </summary>
public class QuotaCheckResult
{
    public bool Allowed { get; set; }
    public decimal CurrentUsage { get; set; }
    public decimal? Limit { get; set; }
    public decimal Remaining { get; set; }
}
