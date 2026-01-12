using System;
using System.Threading.Tasks;
using Grc.Product.Application.Contracts.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Grc.Product.HttpApi.Subscriptions;

/// <summary>
/// REST API controller for subscriptions
/// </summary>
[ApiController]
[Route("api/grc/subscriptions")]
[Authorize]
public class SubscriptionController : AbpControllerBase
{
    private readonly ISubscriptionAppService _subscriptionAppService;

    public SubscriptionController(ISubscriptionAppService subscriptionAppService)
    {
        _subscriptionAppService = subscriptionAppService;
    }

    /// <summary>
    /// Get current tenant's subscription
    /// </summary>
    [HttpGet("current")]
    public async Task<ActionResult<SubscriptionDetailDto>> GetCurrentAsync()
    {
        var subscription = await _subscriptionAppService.GetCurrentSubscriptionAsync();
        return Ok(subscription);
    }

    /// <summary>
    /// Get subscription by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SubscriptionDetailDto>> GetAsync(Guid id)
    {
        var subscription = await _subscriptionAppService.GetAsync(id);
        return Ok(subscription);
    }

    /// <summary>
    /// Subscribe to a product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TenantSubscriptionDto>> SubscribeAsync([FromBody] SubscribeInput input)
    {
        var subscription = await _subscriptionAppService.SubscribeAsync(input);
        return CreatedAtAction(nameof(GetAsync), new { id = subscription.Id }, subscription);
    }

    /// <summary>
    /// Cancel subscription
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<TenantSubscriptionDto>> CancelAsync(Guid id, [FromBody] CancelSubscriptionInput input)
    {
        var subscription = await _subscriptionAppService.CancelAsync(id, input);
        return Ok(subscription);
    }

    /// <summary>
    /// Upgrade subscription
    /// </summary>
    [HttpPost("{id}/upgrade")]
    public async Task<ActionResult<TenantSubscriptionDto>> UpgradeAsync(Guid id, [FromBody] UpgradeSubscriptionInput input)
    {
        var subscription = await _subscriptionAppService.UpgradeAsync(id, input);
        return Ok(subscription);
    }

    /// <summary>
    /// Check quota availability
    /// </summary>
    [HttpPost("quota/check")]
    public async Task<ActionResult<QuotaCheckResult>> CheckQuotaAsync([FromBody] QuotaCheckInput input)
    {
        var result = await _subscriptionAppService.CheckQuotaAsync(input);
        return Ok(result);
    }
}
