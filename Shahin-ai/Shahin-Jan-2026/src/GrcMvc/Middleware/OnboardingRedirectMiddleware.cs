using GrcMvc.Data;
using GrcMvc.Services.Interfaces;
using GrcMvc.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace GrcMvc.Middleware
{
    /// <summary>
    /// Redirects first-time admin users to onboarding wizard if not completed
    /// Based on OnboardingStatus field in Tenant entity
    /// </summary>
    public class OnboardingRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OnboardingRedirectMiddleware> _logger;

        public OnboardingRedirectMiddleware(
            RequestDelegate next,
            ILogger<OnboardingRedirectMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            GrcDbContext dbContext,
            ICurrentTenant currentTenant,
            ITenantContextService tenantContextService)
        {
            // Only check for authenticated users
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }

            // Skip for static files, API calls, onboarding routes, and account routes
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.StartsWith("/api/") || 
                path.StartsWith("/onboardingwizard/") ||
                path.StartsWith("/onboarding/") || 
                path.StartsWith("/trial/") ||
                path.StartsWith("/account/") ||
                path.Contains("/css/") || 
                path.Contains("/js/") || 
                path.Contains("/lib/") ||
                path.Contains("/images/"))
            {
                await _next(context);
                return;
            }

            try
            {
                // Get current tenant ID - prefer ABP's ICurrentTenant, fallback to custom service
                Guid currentTenantId = currentTenant.Id ?? tenantContextService.GetCurrentTenantId();
                if (currentTenantId == Guid.Empty)
                {
                    await _next(context);
                    return;
                }

                // Check if tenant has incomplete onboarding
                var wizard = await dbContext.OnboardingWizards
                    .Where(w => w.TenantId == currentTenantId && !w.IsCompleted)
                    .FirstOrDefaultAsync();

                if (wizard != null)
                {
                    // Get current user ID
                    var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        // Check if user is tenant admin (use RoleConstants for consistency)
                        var tenantUser = await dbContext.TenantUsers
                            .Where(tu => tu.TenantId == currentTenantId && 
                                        tu.UserId == userIdClaim.Value &&
                                        tu.Status == "Active")
                            .FirstOrDefaultAsync();

                        if (tenantUser != null && RoleConstants.IsTenantAdmin(tenantUser.RoleCode))
                        {
                            _logger.LogInformation(
                                "Redirecting tenant admin {UserId} to onboarding wizard (TenantId: {TenantId}, Step: {Step})",
                                userIdClaim.Value, currentTenantId, wizard.CurrentStep);
                            
                            // Redirect with tenantId parameter (not step) - wizard will determine current step
                            context.Response.Redirect($"/OnboardingWizard/Index?tenantId={currentTenantId}");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnboardingRedirectMiddleware");
                // Continue to next middleware on error
            }

            await _next(context);
        }
    }
}
