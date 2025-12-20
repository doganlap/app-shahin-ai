using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Grc.Product.Subscriptions;

namespace Grc.Product.HttpApi.Subscriptions;

[Authorize]
[RemoteService(Name = "Subscription")]
[Route("api/grc/subscriptions")]
public class SubscriptionController : AbpControllerBase, ISubscriptionAppService
{
    private readonly ISubscriptionAppService _subscriptionAppService;

    public SubscriptionController(ISubscriptionAppService subscriptionAppService)
    {
        _subscriptionAppService = subscriptionAppService;
    }

    [HttpGet("current")]
    public virtual Task<SubscriptionDetailDto> GetCurrentSubscriptionAsync()
    {
        return _subscriptionAppService.GetCurrentSubscriptionAsync();
    }

    [HttpPost("subscribe")]
    public virtual Task<TenantSubscriptionDto> SubscribeAsync(SubscribeInput input)
    {
        return _subscriptionAppService.SubscribeAsync(input);
    }

    [HttpPost("{id}/cancel")]
    public virtual Task<TenantSubscriptionDto> CancelAsync(Guid id, [FromBody] CancelSubscriptionInput input)
    {
        return _subscriptionAppService.CancelAsync(id, input);
    }

    [HttpPost("upgrade")]
    public virtual Task<TenantSubscriptionDto> UpgradeAsync(UpgradeSubscriptionInput input)
    {
        return _subscriptionAppService.UpgradeAsync(input);
    }

    [HttpGet("quota-usage")]
    public virtual Task<List<QuotaUsageDto>> GetQuotaUsageAsync([FromQuery] string quotaType = null)
    {
        return _subscriptionAppService.GetQuotaUsageAsync(quotaType);
    }

    [HttpPost("check-quota")]
    public virtual Task<QuotaCheckResult> CheckQuotaAsync([FromBody] CheckQuotaRequest request)
    {
        return _subscriptionAppService.CheckQuotaAsync(request.QuotaType, request.RequestedAmount);
    }
}

public class CheckQuotaRequest
{
    public string QuotaType { get; set; }
    public decimal RequestedAmount { get; set; }
}


