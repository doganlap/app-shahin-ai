using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Onboarding;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Identity;
using Grc.Permissions;

namespace Grc.Services;

/// <summary>
/// Application service for managing user onboarding
/// </summary>
[Authorize]
public class OnboardingAppService : ApplicationService
{
    private readonly IRepository<UserOnboarding, Guid> _onboardingRepository;
    private readonly IRepository<OnboardingTemplate, Guid> _templateRepository;
    private readonly OnboardingManager _onboardingManager;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IUserInitializationService _userInitializationService;

    public OnboardingAppService(
        IRepository<UserOnboarding, Guid> repository,
        IRepository<OnboardingTemplate, Guid> templateRepository,
        OnboardingManager onboardingManager,
        IIdentityUserRepository userRepository,
        IUserInitializationService userInitializationService)
    {
        _onboardingRepository = repository;
        _templateRepository = templateRepository;
        _onboardingManager = onboardingManager;
        _userRepository = userRepository;
        _userInitializationService = userInitializationService;
    }

    /// <summary>
    /// Get current user's onboarding status
    /// </summary>
    public virtual async Task<UserOnboarding?> GetMyOnboardingAsync()
    {
        if (!CurrentUser.Id.HasValue)
        {
            return null;
        }

        return await _onboardingRepository
            .FirstOrDefaultAsync(o => o.UserId == CurrentUser.Id.Value);
    }

    /// <summary>
    /// Check if current user needs onboarding
    /// </summary>
    /// <summary>
    /// Checks if the current user needs to complete onboarding.
    /// Returns TRUE if user has no onboarding record OR onboarding is incomplete.
    /// AUTO-CREATES onboarding record ATOMICALLY on first check for new users.
    /// </summary>
    public virtual async Task<bool> NeedsOnboardingAsync()
    {
        if (!CurrentUser.Id.HasValue)
        {
            return false;
        }

        var onboarding = await GetMyOnboardingAsync();
        
        // FIRST LOGIN DETECTION: If no onboarding record exists, create one ATOMICALLY
        if (onboarding == null)
        {
            Logger.LogInformation(
                "====== FIRST LOGIN DETECTED ======\n" +
                "User ID: {UserId}\n" +
                "Username: {UserName}\n" +
                "Email: {Email}\n" +
                "Initializing user ATOMICALLY...",
                CurrentUser.Id,
                CurrentUser.UserName,
                CurrentUser.Email
            );
            
            // Get user details for atomic initialization
            var user = await _userRepository.GetAsync(CurrentUser.Id.Value);
            
            // ATOMIC INITIALIZATION: All records created in ONE transaction
            onboarding = await _userInitializationService.InitializeNewUserAsync(
                userId: CurrentUser.Id.Value,
                tenantId: CurrentTenant.Id,
                userEmail: user.Email ?? CurrentUser.Email ?? "unknown",
                userName: user.UserName ?? CurrentUser.UserName ?? "unknown"
            );
            
            Logger.LogInformation(
                "✓✓✓ USER ATOMICALLY INITIALIZED ✓✓✓\n" +
                "Onboarding ID: {OnboardingId}\n" +
                "Status: {Status}\n" +
                "All records created in single transaction",
                onboarding.Id,
                onboarding.Status
            );
            
            // New user ALWAYS needs onboarding
            return true;
        }
        
        var needsOnboarding = onboarding.Status == OnboardingStatus.Pending || 
                              onboarding.Status == OnboardingStatus.InProgress;
        
        if (needsOnboarding)
        {
            Logger.LogDebug(
                "User {UserId} needs onboarding. Status: {Status}, CurrentStep: {Step}, CompletedSteps: {CompletedCount}/7",
                CurrentUser.Id,
                onboarding.Status,
                onboarding.CurrentStep,
                onboarding.CompletedSteps?.Count ?? 0
            );
        }
        
        return needsOnboarding;
    }

    /// <summary>
    /// Start onboarding for current user
    /// </summary>
    public virtual async Task<UserOnboarding> StartOnboardingAsync(Guid? templateId = null)
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new UnauthorizedAccessException("User must be authenticated");
        }

        // Check if onboarding already exists
        var existing = await _onboardingRepository
            .FirstOrDefaultAsync(o => o.UserId == CurrentUser.Id.Value);
        
        if (existing != null)
        {
            if (existing.Status == OnboardingStatus.Completed)
            {
                throw new InvalidOperationException("User has already completed onboarding");
            }
            
            existing.Start();
            await _onboardingRepository.UpdateAsync(existing);
            return existing;
        }

        // Create new onboarding
        var onboarding = new UserOnboarding(
            GuidGenerator.Create(),
            CurrentUser.Id.Value,
            CurrentTenant.Id
        );

        // Load template if specified
        OnboardingTemplate? template = null;
        if (templateId.HasValue)
        {
            template = await _templateRepository.GetAsync(templateId.Value);
        }

        // Initialize user with template
        var user = await _userRepository.GetAsync(CurrentUser.Id.Value);
        await _onboardingManager.InitializeUserAsync(onboarding, user, template);

        onboarding.Start();
        await _onboardingRepository.InsertAsync(onboarding);
        
        return onboarding;
    }

    /// <summary>
    /// Complete an onboarding step
    /// </summary>
    public virtual async Task CompleteStepAsync(OnboardingStep step)
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new UnauthorizedAccessException("User must be authenticated");
        }

        var onboarding = await _onboardingRepository
            .FirstOrDefaultAsync(o => o.UserId == CurrentUser.Id.Value);

        if (onboarding == null)
        {
            throw new InvalidOperationException("Onboarding not found");
        }

        onboarding.CompleteStep(step);
        await _onboardingRepository.UpdateAsync(onboarding);
    }

    /// <summary>
    /// Set user preference during onboarding
    /// </summary>
    public virtual async Task SetPreferenceAsync(string key, string value)
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new UnauthorizedAccessException("User must be authenticated");
        }

        var onboarding = await _onboardingRepository
            .FirstOrDefaultAsync(o => o.UserId == CurrentUser.Id.Value);

        if (onboarding == null)
        {
            throw new InvalidOperationException("Onboarding not found");
        }

        onboarding.SetPreference(key, value);
        await _onboardingRepository.UpdateAsync(onboarding);
    }

    /// <summary>
    /// Get available onboarding templates
    /// </summary>
    public virtual async Task<List<OnboardingTemplate>> GetTemplatesAsync()
    {
        return await _templateRepository
            .GetListAsync(t => t.IsActive);
    }

    /// <summary>
    /// Get onboarding for specific user (admin only)
    /// </summary>
    [Authorize(GrcPermissions.Admin.Default)]
    public virtual async Task<UserOnboarding?> GetUserOnboardingAsync(Guid userId)
    {
        return await _onboardingRepository
            .FirstOrDefaultAsync(o => o.UserId == userId);
    }

    /// <summary>
    /// Skip onboarding for a user (admin only)
    /// </summary>
    [Authorize(GrcPermissions.Admin.Default)]
    public virtual async Task SkipOnboardingAsync(Guid userId, string reason)
    {
        var onboarding = await _onboardingRepository
            .FirstOrDefaultAsync(o => o.UserId == userId);

        if (onboarding == null)
        {
            // Create and skip
            onboarding = new UserOnboarding(GuidGenerator.Create(), userId, CurrentTenant.Id);
            onboarding.Skip(reason);
            await _onboardingRepository.InsertAsync(onboarding);
        }
        else
        {
            onboarding.Skip(reason);
            await _onboardingRepository.UpdateAsync(onboarding);
        }
    }

    /// <summary>
    /// Validates that the current user is fully initialized and ready for productive work.
    /// Checks onboarding completion, roles, permissions, and profile completeness.
    /// </summary>
    /// <returns>True if user is fully productive, false otherwise</returns>
    public virtual async Task<bool> ValidateUserIsProductiveAsync()
    {
        if (!CurrentUser.Id.HasValue)
        {
            Logger.LogWarning("ValidateUserIsProductiveAsync called without authenticated user");
            return false;
        }

        try
        {
            var isProductive = await _userInitializationService.ValidateUserIsProductiveAsync(CurrentUser.Id.Value);
            
            Logger.LogInformation(
                "User {UserId} productive validation result: {IsProductive}", 
                CurrentUser.Id.Value, 
                isProductive
            );
            
            return isProductive;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating user {UserId} is productive", CurrentUser.Id.Value);
            return false;
        }
    }

    /// <summary>
    /// Gets detailed initialization status for the current user.
    /// Returns information about missing components preventing productivity.
    /// </summary>
    /// <returns>Detailed status including missing components</returns>
    public virtual async Task<UserInitializationStatus> GetInitializationStatusAsync()
    {
        if (!CurrentUser.Id.HasValue)
        {
            return new UserInitializationStatus
            {
                IsFullyInitialized = false,
                HasOnboardingRecord = false,
                OnboardingIsComplete = false,
                HasRolesAssigned = false,
                HasPermissions = false,
                ProfileIsComplete = false,
                MissingComponents = new[] { "User not authenticated" },
                LastValidationTime = Clock.Now
            };
        }

        try
        {
            var status = await _userInitializationService.GetInitializationStatusAsync(CurrentUser.Id.Value);
            
            if (!status.IsFullyInitialized)
            {
                Logger.LogWarning(
                    "User {UserId} initialization incomplete. Missing: {MissingComponents}", 
                    CurrentUser.Id.Value, 
                    string.Join(", ", status.MissingComponents)
                );
            }
            
            return status;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting initialization status for user {UserId}", CurrentUser.Id.Value);
            return new UserInitializationStatus
            {
                IsFullyInitialized = false,
                MissingComponents = new[] { "Error retrieving status" },
                LastValidationTime = Clock.Now
            };
        }
    }
}
