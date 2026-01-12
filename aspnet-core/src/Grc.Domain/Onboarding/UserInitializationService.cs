using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Grc.Onboarding;

/// <summary>
/// ATOMIC USER INITIALIZATION SERVICE
/// Ensures ALL user-related records are created together in ONE transaction.
/// Prevents broken states, orphaned records, and incomplete configurations.
/// </summary>
public class UserInitializationService : DomainService, IUserInitializationService
{
    private readonly IRepository<UserOnboarding, Guid> _onboardingRepository;
    private readonly IRepository<OnboardingTemplate, Guid> _templateRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IPermissionManager _permissionManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public UserInitializationService(
        IRepository<UserOnboarding, Guid> onboardingRepository,
        IRepository<OnboardingTemplate, Guid> templateRepository,
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IPermissionManager permissionManager,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _onboardingRepository = onboardingRepository;
        _templateRepository = templateRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionManager = permissionManager;
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// ATOMIC OPERATION: Initialize new user with ALL required records.
    /// MUST be called within a Unit of Work transaction for atomicity.
    /// </summary>
    [UnitOfWork]
    public virtual async Task<UserOnboarding> InitializeNewUserAsync(
        Guid userId, 
        Guid? tenantId, 
        string userEmail, 
        string userName)
    {
        Logger.LogInformation(
            "====== ATOMIC USER INITIALIZATION START ======\n" +
            "User ID: {UserId}\n" +
            "Tenant ID: {TenantId}\n" +
            "Email: {Email}\n" +
            "Username: {UserName}",
            userId, tenantId, userEmail, userName
        );

        try
        {
            // STEP 1: Validate user exists
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw new BusinessException($"User with ID {userId} not found. Cannot initialize.");
            }

            Logger.LogInformation("✓ User exists in database");

            // STEP 2: Check for existing onboarding (prevent duplicates)
            var existingOnboarding = await _onboardingRepository
                .FirstOrDefaultAsync(o => o.UserId == userId);
            
            if (existingOnboarding != null)
            {
                Logger.LogWarning(
                    "User {UserId} already has onboarding record (ID: {OnboardingId}, Status: {Status}). Returning existing.",
                    userId, existingOnboarding.Id, existingOnboarding.Status
                );
                return existingOnboarding;
            }

            // STEP 3: Create onboarding record
            var onboarding = new UserOnboarding(
                GuidGenerator.Create(),
                userId,
                tenantId
            );

            onboarding.Start(); // Set status to InProgress
            
            Logger.LogInformation("✓ Created UserOnboarding record (ID: {OnboardingId})", onboarding.Id);

            // STEP 4: Get default template for new users
            var defaultTemplate = await _templateRepository
                .FirstOrDefaultAsync(t => t.IsActive);

            if (defaultTemplate != null)
            {
                Logger.LogInformation(
                    "✓ Found default template: {TemplateName} (ID: {TemplateId})",
                    defaultTemplate.Name,
                    defaultTemplate.Id
                );

                // Apply template defaults (no TemplateId property on UserOnboarding)
                
                // Copy default roles from template
                if (defaultTemplate.DefaultRoles != null && defaultTemplate.DefaultRoles.Any())
                {
                    foreach (var roleName in defaultTemplate.DefaultRoles)
                    {
                        onboarding.AssignRole(roleName);
                        Logger.LogInformation("  → Assigned default role: {RoleName}", roleName);
                    }
                }

                // Copy default features from template
                if (defaultTemplate.DefaultFeatures != null && defaultTemplate.DefaultFeatures.Any())
                {
                    foreach (var feature in defaultTemplate.DefaultFeatures)
                    {
                        onboarding.SetFeature(feature.Key, feature.Value);
                        Logger.LogInformation("  → Enabled default feature: {Feature} = {Value}", feature.Key, feature.Value);
                    }
                }
            }
            else
            {
                Logger.LogWarning("⚠ No default template found. User will select roles during onboarding.");
            }

            // STEP 5: Set initial preferences
            onboarding.SetPreference("Email", userEmail);
            onboarding.SetPreference("UserName", userName);
            onboarding.SetPreference("InitializedAt", Clock.Now.ToString("O"));
            onboarding.SetPreference("InitializationMethod", "Atomic");
            
            Logger.LogInformation("✓ Set initial preferences");

            // STEP 6: Insert onboarding record (atomic within UoW)
            await _onboardingRepository.InsertAsync(onboarding, autoSave: false); // Don't save yet - wait for transaction
            
            Logger.LogInformation("✓ Inserted onboarding record into repository (pending transaction commit)");

            // STEP 7: Save all changes atomically
            await _unitOfWorkManager.Current!.SaveChangesAsync();
            
            Logger.LogInformation(
                "✓✓✓ ATOMIC COMMIT SUCCESSFUL ✓✓✓\n" +
                "All records created in single transaction:\n" +
                "  - UserOnboarding: {OnboardingId}\n" +
                "  - Status: {Status}\n" +
                "  - CurrentStep: {Step}\n" +
                "  - AssignedRoles: {RoleCount}\n" +
                "  - EnabledFeatures: {FeatureCount}\n" +
                "  - UserPreferences: {PrefCount}\n" +
                "====== ATOMIC USER INITIALIZATION COMPLETE ======",
                onboarding.Id,
                onboarding.Status,
                onboarding.CurrentStep,
                onboarding.AssignedRoles?.Count ?? 0,
                onboarding.EnabledFeatures?.Count ?? 0,
                onboarding.UserPreferences?.Count ?? 0
            );

            return onboarding;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "✗✗✗ ATOMIC USER INITIALIZATION FAILED ✗✗✗\n" +
                "Transaction will rollback - NO records created.\n" +
                "User: {UserId}, Email: {Email}",
                userId, userEmail
            );
            throw;
        }
    }

    /// <summary>
    /// Validate user is FULLY initialized and ready for productive work.
    /// Checks all components required for a productive workplace.
    /// </summary>
    public virtual async Task<bool> ValidateUserIsProductiveAsync(Guid userId)
    {
        var status = await GetInitializationStatusAsync(userId);
        
        if (!status.IsFullyInitialized)
        {
            Logger.LogWarning(
                "User {UserId} is NOT fully initialized. Missing: {Missing}",
                userId,
                string.Join(", ", status.MissingComponents)
            );
        }
        
        return status.IsFullyInitialized;
    }

    /// <summary>
    /// Get detailed initialization status for troubleshooting.
    /// </summary>
    public virtual async Task<UserInitializationStatus> GetInitializationStatusAsync(Guid userId)
    {
        var status = new UserInitializationStatus
        {
            LastValidationTime = Clock.Now
        };

        var missingComponents = new List<string>();

        // CHECK 1: Onboarding record exists
        var onboarding = await _onboardingRepository
            .FirstOrDefaultAsync(o => o.UserId == userId);
        
        status.HasOnboardingRecord = onboarding != null;
        if (!status.HasOnboardingRecord)
        {
            missingComponents.Add("UserOnboarding record");
        }

        if (onboarding != null)
        {
            // CHECK 2: Onboarding is complete
            status.OnboardingIsComplete = onboarding.Status == OnboardingStatus.Completed;
            if (!status.OnboardingIsComplete)
            {
                missingComponents.Add($"Onboarding completion (current: {onboarding.Status})");
            }

            // CHECK 3: Roles assigned
            status.HasRolesAssigned = onboarding.AssignedRoles != null && onboarding.AssignedRoles.Any();
            if (!status.HasRolesAssigned)
            {
                missingComponents.Add("Role assignments");
            }

            // CHECK 4: Profile complete
            var requiredFields = new[] { "FullName", "JobTitle", "PhoneNumber", "PreferredLanguage", "Timezone" };
            var hasAllFields = requiredFields.All(field => 
                onboarding.UserPreferences != null && 
                onboarding.UserPreferences.ContainsKey(field) &&
                !string.IsNullOrWhiteSpace(onboarding.UserPreferences[field])
            );
            
            status.ProfileIsComplete = hasAllFields;
            if (!status.ProfileIsComplete)
            {
                var missing = requiredFields.Where(field => 
                    onboarding.UserPreferences == null || 
                    !onboarding.UserPreferences.ContainsKey(field) ||
                    string.IsNullOrWhiteSpace(onboarding.UserPreferences[field])
                ).ToArray();
                
                missingComponents.Add($"Profile fields: {string.Join(", ", missing)}");
            }
        }

        // CHECK 5: User has permissions
        try
        {
            var user = await _userRepository.FindAsync(userId);
            if (user != null)
            {
                // Check if user has at least one role with permissions
                status.HasPermissions = user.Roles.Any();
                if (!status.HasPermissions)
                {
                    missingComponents.Add("User roles in Identity system");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not check user permissions for {UserId}", userId);
            status.HasPermissions = false;
            missingComponents.Add("Permission check failed");
        }

        // FINAL STATUS
        status.IsFullyInitialized = 
            status.HasOnboardingRecord &&
            status.OnboardingIsComplete &&
            status.HasRolesAssigned &&
            status.ProfileIsComplete &&
            status.HasPermissions;

        status.MissingComponents = missingComponents.ToArray();

        Logger.LogInformation(
            "User {UserId} initialization status:\n" +
            "  Fully Initialized: {IsReady}\n" +
            "  Has Onboarding Record: {HasRecord}\n" +
            "  Onboarding Complete: {IsComplete}\n" +
            "  Roles Assigned: {HasRoles}\n" +
            "  Profile Complete: {ProfileComplete}\n" +
            "  Has Permissions: {HasPerms}\n" +
            "  Missing: {Missing}",
            userId,
            status.IsFullyInitialized,
            status.HasOnboardingRecord,
            status.OnboardingIsComplete,
            status.HasRolesAssigned,
            status.ProfileIsComplete,
            status.HasPermissions,
            status.MissingComponents.Length > 0 ? string.Join(", ", status.MissingComponents) : "None"
        );

        return status;
    }
}
