using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Grc.Onboarding;

/// <summary>
/// CRITICAL SERVICE: Atomic user initialization ensuring ALL records are created together.
/// Guarantees no broken states, orphaned records, or incomplete user configurations.
/// MUST be called in a database transaction for atomicity.
/// </summary>
public interface IUserInitializationService : IDomainService
{
    /// <summary>
    /// Initialize a new user with ALL required records in ONE atomic transaction.
    /// Creates:
    /// 1. UserOnboarding record
    /// 2. Default roles assignment
    /// 3. Default permissions
    /// 4. Initial preferences
    /// 5. Audit trail entries
    /// 6. Tenant isolation records
    /// 
    /// GUARANTEES:
    /// - Atomic operation (all-or-nothing)
    /// - No orphaned records
    /// - Complete audit trail
    /// - Cross-layer consistency
    /// - Tenant isolation
    /// </summary>
    /// <param name="userId">The user ID to initialize</param>
    /// <param name="tenantId">The tenant ID for multi-tenant isolation</param>
    /// <param name="userEmail">User email for audit trail</param>
    /// <param name="userName">Username for audit trail</param>
    /// <returns>UserOnboarding record with complete initialization</returns>
    Task<UserOnboarding> InitializeNewUserAsync(
        Guid userId, 
        Guid? tenantId, 
        string userEmail, 
        string userName);

    /// <summary>
    /// Validate that a user is FULLY initialized and ready for productive work.
    /// Checks:
    /// - Onboarding record exists
    /// - Onboarding is complete
    /// - Roles are assigned
    /// - Permissions are granted
    /// - Profile is complete
    /// - No broken state
    /// </summary>
    /// <param name="userId">User to validate</param>
    /// <returns>TRUE if user is fully initialized and productive</returns>
    Task<bool> ValidateUserIsProductiveAsync(Guid userId);

    /// <summary>
    /// Get detailed initialization status for troubleshooting.
    /// Returns what's missing if user is not fully initialized.
    /// </summary>
    Task<UserInitializationStatus> GetInitializationStatusAsync(Guid userId);
}

/// <summary>
/// Detailed initialization status for a user
/// </summary>
public class UserInitializationStatus
{
    public bool IsFullyInitialized { get; set; }
    public bool HasOnboardingRecord { get; set; }
    public bool OnboardingIsComplete { get; set; }
    public bool HasRolesAssigned { get; set; }
    public bool HasPermissions { get; set; }
    public bool ProfileIsComplete { get; set; }
    public string[] MissingComponents { get; set; } = Array.Empty<string>();
    public DateTime? LastValidationTime { get; set; }
}
