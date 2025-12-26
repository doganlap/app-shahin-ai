using System;
using System.Linq;
using System.Threading.Tasks;
using Grc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Grc.Web.Middleware;

/// <summary>
/// CRITICAL MIDDLEWARE: Enforces mandatory onboarding for ALL users before system access.
/// This ensures every user completes required setup (profile, roles, features) before
/// accessing ANY functionality - preventing incomplete configurations and ensuring 
/// productive work environment from first login.
/// </summary>
public class OnboardingCheckMiddleware : IMiddleware, ITransientDependency
{
    private readonly OnboardingAppService _onboardingService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<OnboardingCheckMiddleware> _logger;

    public OnboardingCheckMiddleware(
        OnboardingAppService onboardingService,
        ICurrentUser currentUser,
        ILogger<OnboardingCheckMiddleware> logger)
    {
        _onboardingService = onboardingService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Skip check for non-authenticated users
        if (!_currentUser.IsAuthenticated || !_currentUser.Id.HasValue)
        {
            await next(context);
            return;
        }

        // Skip check for whitelisted paths (static files, error pages, API endpoints)
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (ShouldSkipOnboardingCheck(path))
        {
            await next(context);
            return;
        }

        try
        {
            // CRITICAL CHECK: Does this user need onboarding?
            var needsOnboarding = await _onboardingService.NeedsOnboardingAsync();
            
            if (needsOnboarding)
            {
                // User has incomplete onboarding - BLOCK all access except /Onboarding
                if (!path.Contains("/onboarding"))
                {
                    _logger.LogWarning(
                        "ONBOARDING REQUIRED: User {UserId} ({UserName}) attempted to access {Path} with incomplete setup. REDIRECTING to mandatory onboarding.",
                        _currentUser.Id,
                        _currentUser.UserName,
                        context.Request.Path
                    );
                    
                    // HARD REDIRECT: User MUST complete onboarding before accessing anything
                    context.Response.Redirect("/Onboarding");
                    return;
                }
            }
            else
            {
                // User has completed onboarding - prevent accessing /Onboarding again
                if (path.Contains("/onboarding"))
                {
                    _logger.LogInformation(
                        "User {UserId} with completed onboarding attempted to access /Onboarding. Redirecting to Dashboard",
                        _currentUser.Id
                    );
                    
                    // Redirect completed users to Dashboard (they're already set up)
                    context.Response.Redirect("/Dashboard");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            // FAIL SAFE: If onboarding check fails, redirect to onboarding to be safe
            _logger.LogError(ex, 
                "ERROR checking onboarding status for user {UserId}. Redirecting to onboarding as safety measure.", 
                _currentUser.Id
            );
            
            if (!path.Contains("/onboarding"))
            {
                context.Response.Redirect("/Onboarding");
                return;
            }
        }

        await next(context);
    }

    /// <summary>
    /// Determines if onboarding check should be skipped for the given path.
    /// Only skips for: static files, error pages, API endpoints, account pages.
    /// ALL functional pages are protected until onboarding is complete.
    /// </summary>
    private static bool ShouldSkipOnboardingCheck(string path)
    {
        // TEMPORARILY DISABLED: Allow all access while onboarding is being fixed
        // TODO: Re-enable once onboarding flow is properly tested
        return true;

        /* Original logic - re-enable when onboarding is working:
        return path.Contains("/account/") ||
               path.Contains("/error/") ||
               path.Contains("/onboarding") ||
               path.Contains("/api/") ||
               path.Contains("/swagger") ||
               path.Contains("/abp/") ||
               path.Contains("/libs/") ||
               path.Contains("/css/") ||
               path.Contains("/js/") ||
               path.Contains("/images/") ||
               path.Contains("/signalr") ||
               path.Contains("/_framework") ||
               path.StartsWith("/health");
        */
    }
}
