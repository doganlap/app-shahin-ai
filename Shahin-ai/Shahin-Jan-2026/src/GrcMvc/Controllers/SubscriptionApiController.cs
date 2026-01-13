using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GrcMvc.Models.DTOs;
using GrcMvc.Models;
using GrcMvc.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace GrcMvc.Controllers
{
    /// <summary>
    /// Subscription API Controller
    /// Handles REST API requests for subscription management, plan selection, and billing
    /// Routes: /api/subscriptions and /api/subscription (for frontend compatibility)
    /// </summary>
    [Route("api/[controller]")]
    [Route("api/subscription")] // Alias for frontend compatibility
    [ApiController]
    [Authorize]
    public class SubscriptionApiController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ITenantContextService _tenantContext;
        private readonly ILogger<SubscriptionApiController> _logger;

        public SubscriptionApiController(
            ISubscriptionService subscriptionService,
            ITenantContextService tenantContext,
            ILogger<SubscriptionApiController> logger)
        {
            _subscriptionService = subscriptionService;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        private Guid GetCurrentTenantId() => _tenantContext.GetCurrentTenantId();

        /// <summary>
        /// Get current tenant's subscription
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSubscriptions(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string order = "asc",
            [FromQuery] string? status = null,
            [FromQuery] string? q = null)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                var subscription = await _subscriptionService.GetSubscriptionByTenantAsync(tenantId);

                // Return as paginated list for frontend compatibility
                var subscriptions = subscription != null
                    ? new List<SubscriptionDto> { subscription }
                    : new List<SubscriptionDto>();

                // Apply status filter if provided
                if (!string.IsNullOrEmpty(status))
                {
                    subscriptions = subscriptions.Where(s => 
                        s.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var response = new PaginatedResponse<SubscriptionDto>
                {
                    Items = subscriptions,
                    Page = page,
                    Size = size,
                    TotalItems = subscriptions.Count
                };

                return Ok(ApiResponse<PaginatedResponse<SubscriptionDto>>.SuccessResponse(
                    response, "Subscriptions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscriptions");
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred retrieving subscriptions."));
            }
        }

        /// <summary>
        /// Bulk create subscriptions
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreateSubscriptions([FromBody] BulkOperationRequest bulkRequest)
        {
            try
            {
                if (bulkRequest?.Items == null || bulkRequest.Items.Count == 0)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Items are required for bulk operation"));

                int successCount = 0;
                int failedCount = 0;

                foreach (var item in bulkRequest.Items)
                {
                    try
                    {
                        var tenantId = Guid.Parse(item["tenantId"]?.ToString() ?? Guid.NewGuid().ToString());
                        var planId = Guid.Parse(item["planId"]?.ToString() ?? Guid.NewGuid().ToString());
                        var billingCycle = item["billingCycle"]?.ToString() ?? "Monthly";
                        
                        await _subscriptionService.CreateSubscriptionAsync(tenantId, planId, billingCycle);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to create subscription in bulk operation");
                        failedCount++;
                    }
                }

                var result = new BulkOperationResult
                {
                    TotalItems = bulkRequest.Items.Count,
                    SuccessfulItems = successCount,
                    FailedItems = failedCount,
                    CompletedAt = DateTime.UtcNow
                };

                return Ok(ApiResponse<BulkOperationResult>.SuccessResponse(result, "Bulk operation completed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk subscription creation");
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred processing bulk operation."));
            }
        }

        /// <summary>
        /// Get subscription by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscription(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid subscription ID"));

                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
                
                if (subscription == null)
                    return NotFound(ApiResponse<object>.ErrorResponse($"Subscription {id} not found"));

                return Ok(ApiResponse<SubscriptionDto>.SuccessResponse(subscription, "Subscription retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred retrieving subscription."));
            }
        }

        /// <summary>
        /// Subscribe to a plan
        /// Creates a new subscription for a tenant
        /// </summary>
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest subscriptionData)
        {
            try
            {
                if (subscriptionData == null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Subscription data is required"));

                var tenantId = subscriptionData.TenantId ?? GetCurrentTenantId();
                var planId = subscriptionData.PlanId;
                var billingCycle = subscriptionData.BillingCycle ?? "Monthly";

                if (planId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Plan ID is required"));

                var newSubscription = await _subscriptionService.CreateSubscriptionAsync(tenantId, planId, billingCycle);

                _logger.LogInformation("Subscription created: {Id} for tenant {TenantId}", newSubscription.Id, tenantId);

                return CreatedAtAction(nameof(GetSubscription), new { id = newSubscription.Id },
                    ApiResponse<SubscriptionDto>.SuccessResponse(newSubscription, "Subscription created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription");
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred creating subscription."));
            }
        }

        /// <summary>
        /// Cancel subscription
        /// Cancels an active subscription
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelSubscription([FromBody] CancelSubscriptionRequest cancelData)
        {
            try
            {
                if (cancelData == null || cancelData.SubscriptionId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Subscription ID is required"));

                var success = await _subscriptionService.CancelSubscriptionAsync(
                    cancelData.SubscriptionId, 
                    cancelData.Reason ?? "User requested");

                if (!success)
                    return NotFound(ApiResponse<object>.ErrorResponse("Subscription not found or already cancelled"));

                var cancelResult = new
                {
                    subscriptionId = cancelData.SubscriptionId,
                    status = "Cancelled",
                    cancelledDate = DateTime.UtcNow,
                    reason = cancelData.Reason ?? "User requested",
                    message = "Subscription cancelled successfully"
                };

                return Ok(ApiResponse<object>.SuccessResponse(cancelResult, "Subscription cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription");
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred cancelling subscription."));
            }
        }

        /// <summary>
        /// Upgrade subscription
        /// Upgrades subscription to a higher tier plan
        /// </summary>
        [HttpPost("upgrade")]
        public async Task<IActionResult> UpgradeSubscription([FromBody] UpgradeSubscriptionRequest upgradeData)
        {
            try
            {
                if (upgradeData == null || upgradeData.SubscriptionId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Upgrade data is required"));

                // Get current subscription
                var currentSubscription = await _subscriptionService.GetSubscriptionByIdAsync(upgradeData.SubscriptionId);
                if (currentSubscription == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Subscription not found"));

                // Get new plan details
                var newPlan = await _subscriptionService.GetPlanByIdAsync(upgradeData.NewPlanId);
                if (newPlan == null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("New plan not found"));

                // Create new subscription with upgraded plan (this cancels the old one internally)
                var tenantId = currentSubscription.TenantId ?? GetCurrentTenantId();
                var newSubscription = await _subscriptionService.CreateSubscriptionAsync(
                    tenantId, 
                    upgradeData.NewPlanId, 
                    currentSubscription.BillingCycle);

                // Cancel old subscription
                await _subscriptionService.CancelSubscriptionAsync(upgradeData.SubscriptionId, "Upgraded to new plan");

                var upgradeResult = new
                {
                    subscriptionId = newSubscription.Id,
                    previousPlanId = currentSubscription.PlanId,
                    newPlanId = upgradeData.NewPlanId,
                    newPlanName = newPlan.Name,
                    upgradedDate = DateTime.UtcNow,
                    message = "Subscription upgraded successfully"
                };

                return Ok(ApiResponse<object>.SuccessResponse(upgradeResult, "Subscription upgraded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upgrading subscription");
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred upgrading subscription."));
            }
        }

        /// <summary>
        /// Get available subscription plans
        /// Returns all available plans with pricing and features
        /// </summary>
        [HttpGet("plans")]
        [AllowAnonymous] // Plans should be publicly viewable
        public async Task<IActionResult> GetSubscriptionPlans()
        {
            try
            {
                var plans = await _subscriptionService.GetAllPlansAsync();

                return Ok(ApiResponse<List<SubscriptionPlanDto>>.SuccessResponse(
                    plans, "Subscription plans retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans");
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred retrieving plans."));
            }
        }

        /// <summary>
        /// Update subscription
        /// Updates subscription details (full update)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubscription(Guid id, [FromBody] UpdateSubscriptionRequest updateData)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid subscription ID"));

                if (updateData == null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Subscription data is required"));

                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
                if (subscription == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Subscription not found"));

                // Update status if provided
                if (!string.IsNullOrEmpty(updateData.Status))
                {
                    subscription = await _subscriptionService.UpdateSubscriptionStatusAsync(id, updateData.Status);
                }

                var updatedSubscription = new
                {
                    id = subscription.Id,
                    tenantId = subscription.TenantId,
                    planId = subscription.PlanId,
                    status = subscription.Status,
                    billingCycle = subscription.BillingCycle,
                    autoRenewal = subscription.AutoRenew,
                    updatedDate = DateTime.UtcNow,
                    message = "Subscription updated successfully"
                };

                return Ok(ApiResponse<object>.SuccessResponse(updatedSubscription, "Subscription updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred updating subscription."));
            }
        }

        /// <summary>
        /// Partially update subscription
        /// Updates specific fields of a subscription (partial update)
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchSubscription(Guid id, [FromBody] PatchSubscriptionRequest patchData)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid subscription ID"));

                if (patchData == null)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Patch data is required"));

                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
                if (subscription == null)
                    return NotFound(ApiResponse<object>.ErrorResponse("Subscription not found"));

                // Apply status update if provided
                if (!string.IsNullOrEmpty(patchData.Status))
                {
                    subscription = await _subscriptionService.UpdateSubscriptionStatusAsync(id, patchData.Status);
                }

                // Handle activation
                if (patchData.Activate == true)
                {
                    await _subscriptionService.ActivateSubscriptionAsync(id);
                    subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
                }

                // Handle renewal
                if (patchData.Renew == true)
                {
                    await _subscriptionService.RenewSubscriptionAsync(id);
                    subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
                }

                var patchedSubscription = new
                {
                    id = subscription?.Id,
                    status = subscription?.Status,
                    autoRenew = subscription?.AutoRenew,
                    billingCycle = subscription?.BillingCycle,
                    updatedDate = DateTime.UtcNow,
                    message = "Subscription updated successfully"
                };

                return Ok(ApiResponse<object>.SuccessResponse(patchedSubscription, "Subscription updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error patching subscription {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred updating subscription."));
            }
        }

    /// <summary>
    /// Delete subscription
    /// Permanently removes a subscription
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubscription(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid subscription ID"));

            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            if (subscription == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Subscription not found"));

            // Cancel the subscription (soft delete - we don't permanently delete subscriptions)
            var success = await _subscriptionService.CancelSubscriptionAsync(id, "Deleted by user");
            
            if (!success)
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to delete subscription"));

            var deleteResult = new
            {
                subscriptionId = id,
                status = "Deleted",
                deletedDate = DateTime.UtcNow,
                message = "Subscription deleted successfully"
            };

            return Ok(ApiResponse<object>.SuccessResponse(deleteResult, "Subscription deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subscription {Id}", id);
            return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred deleting subscription."));
        }
    }

    /// <summary>
    /// Get available subscription plans
    /// Alias for /api/subscriptions/plans to match frontend expectations
    /// </summary>
    [HttpGet("available-plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailablePlans()
    {
        return await GetSubscriptionPlans();
    }

    /// <summary>
    /// Change subscription plan
    /// Changes an active subscription to a different plan
    /// </summary>
    [HttpPost("{subscriptionId}/change-plan")]
    public async Task<IActionResult> ChangePlan(Guid subscriptionId, [FromBody] ChangePlanRequest changePlanData)
    {
        try
        {
            if (subscriptionId == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid subscription ID"));

            if (changePlanData == null || changePlanData.NewPlanId == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorResponse("New plan ID is required"));

            var currentSubscription = await _subscriptionService.GetSubscriptionByIdAsync(subscriptionId);
            if (currentSubscription == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Subscription not found"));

            var newPlan = await _subscriptionService.GetPlanByIdAsync(changePlanData.NewPlanId);
            if (newPlan == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("New plan not found"));

            // Create new subscription with new plan
            var tenantId = currentSubscription.TenantId ?? GetCurrentTenantId();
            var newSubscription = await _subscriptionService.CreateSubscriptionAsync(
                tenantId,
                changePlanData.NewPlanId,
                currentSubscription.BillingCycle);

            // Cancel old subscription
            await _subscriptionService.CancelSubscriptionAsync(subscriptionId, "Plan changed");

            var changeResult = new
            {
                subscriptionId = newSubscription.Id,
                previousPlanId = currentSubscription.PlanId,
                newPlanId = changePlanData.NewPlanId,
                newPlanName = newPlan.Name,
                changedDate = DateTime.UtcNow,
                effectiveDate = DateTime.UtcNow,
                message = "Plan changed successfully"
            };

            return Ok(ApiResponse<object>.SuccessResponse(changeResult, "Plan changed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing plan for subscription {Id}", subscriptionId);
            return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred changing plan."));
        }
    }

    /// <summary>
    /// Cancel subscription by ID
    /// Alias for /api/subscriptions/cancel to match frontend expectations
    /// </summary>
    [HttpPost("{subscriptionId}/cancel")]
    public async Task<IActionResult> CancelSubscriptionById(Guid subscriptionId, [FromBody] CancelSubscriptionRequest? cancelData = null)
    {
        try
        {
            if (subscriptionId == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid subscription ID"));

            var reason = cancelData?.Reason ?? "User requested";
            var success = await _subscriptionService.CancelSubscriptionAsync(subscriptionId, reason);

            if (!success)
                return NotFound(ApiResponse<object>.ErrorResponse("Subscription not found or already cancelled"));

            var cancelResult = new
            {
                subscriptionId = subscriptionId,
                status = "Cancelled",
                cancelledDate = DateTime.UtcNow,
                reason = reason,
                message = "Subscription cancelled successfully. Access will continue until the end of the billing period."
            };

            return Ok(ApiResponse<object>.SuccessResponse(cancelResult, "Subscription cancelled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling subscription {Id}", subscriptionId);
            return BadRequest(ApiResponse<object>.ErrorResponse("An error occurred cancelling subscription."));
        }
    }
}

// Request DTOs for Subscription API
public class SubscribeRequest
{
    public Guid? TenantId { get; set; }
    public Guid PlanId { get; set; }
    public string? BillingCycle { get; set; }
}

public class CancelSubscriptionRequest
{
    public Guid SubscriptionId { get; set; }
    public string? Reason { get; set; }
}

public class UpgradeSubscriptionRequest
{
    public Guid SubscriptionId { get; set; }
    public Guid NewPlanId { get; set; }
}

public class UpdateSubscriptionRequest
{
    public string? Status { get; set; }
    public string? BillingCycle { get; set; }
    public bool? AutoRenewal { get; set; }
}

public class PatchSubscriptionRequest
{
    public string? Status { get; set; }
    public string? BillingCycle { get; set; }
    public bool? AutoRenewal { get; set; }
    public bool? Activate { get; set; }
    public bool? Renew { get; set; }
}

public class ChangePlanRequest
{
    public Guid NewPlanId { get; set; }
}
}
