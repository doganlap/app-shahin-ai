using System;
using System.Threading.Tasks;
using Grc.Onboarding;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Grc.Services;

/// <summary>
/// Domain service for managing user onboarding process
/// </summary>
public class OnboardingManager : DomainService, ITransientDependency
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IPermissionManager _permissionManager;
    
    public OnboardingManager(
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IPermissionManager permissionManager)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionManager = permissionManager;
    }
    
    /// <summary>
    /// Initialize user with default permissions and roles
    /// </summary>
    [UnitOfWork]
    public virtual async Task InitializeUserAsync(
        UserOnboarding onboarding,
        IdentityUser user,
        OnboardingTemplate? template = null)
    {
        // Assign default roles from template
        if (template != null)
        {
            foreach (var roleName in template.DefaultRoles)
            {
                var role = await _roleRepository.FindByNormalizedNameAsync(roleName.ToUpperInvariant());
                if (role != null)
                {
                    user.AddRole(role.Id);
                    onboarding.AssignRole(roleName);
                }
            }
        }
        
        // Set default features
        if (template != null)
        {
            foreach (var feature in template.DefaultFeatures)
            {
                onboarding.SetFeature(feature.Key, feature.Value);
            }
        }
        
        // Set user preferences
        var language = user.ExtraProperties.ContainsKey("Language") 
            ? user.ExtraProperties["Language"]?.ToString() ?? "en"
            : "en";
        onboarding.SetPreference("Language", language);
        onboarding.SetPreference("Theme", "light");
        onboarding.SetPreference("ShowWelcomeTour", "true");
        
        await _userRepository.UpdateAsync(user);
    }
    
    /// <summary>
    /// Grant permissions to user based on assigned roles
    /// </summary>
    [UnitOfWork]
    public virtual async Task GrantPermissionsAsync(Guid userId, string[] permissions)
    {
        foreach (var permission in permissions)
        {
            await _permissionManager.SetAsync(permission, "U", userId.ToString(), true);
        }
    }
    
    /// <summary>
    /// Validate onboarding can proceed
    /// </summary>
    public virtual Task<bool> ValidateOnboardingAsync(UserOnboarding onboarding)
    {
        // Check if all required steps are completed
        if (onboarding.Status != OnboardingStatus.InProgress)
        {
            return Task.FromResult(false);
        }
        
        // Validate business rules
        if (onboarding.AssignedRoles.Count == 0)
        {
            return Task.FromResult(false);
        }
        
        return Task.FromResult(true);
    }
}
