using System;
using System.Linq;
using System.Threading.Tasks;
using GrcMvc.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Identity;
using AbpIdentityUser = Volo.Abp.Identity.IdentityUser;
using AbpIdentityRole = Volo.Abp.Identity.IdentityRole;

namespace GrcMvc.Services.Implementations
{
    /// <summary>
    /// Agent service for creating tenants with admin users using ABP Framework patterns
    /// Follows ABP conventions to avoid customizations and hiccups
    /// </summary>
    public class TenantCreationAgentService : ITenantCreationAgentService, ITransientDependency
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly ITenantManager _tenantManager;
        private readonly IdentityUserManager _userManager;
        private readonly IdentityRoleManager _roleManager;
        private readonly ILogger<TenantCreationAgentService> _logger;

        public TenantCreationAgentService(
            ITenantRepository tenantRepository,
            ICurrentTenant currentTenant,
            ITenantManager tenantManager,
            IdentityUserManager userManager,
            IdentityRoleManager roleManager,
            ILogger<TenantCreationAgentService> logger)
        {
            _tenantRepository = tenantRepository;
            _currentTenant = currentTenant;
            _tenantManager = tenantManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<Guid> CreateTenantWithAdminAsync(string tenantName, string adminEmail, string adminPassword)
        {
            Tenant? tenant = null;
            AbpIdentityUser? user = null;

            try
            {
                // Step 1: Create ABP tenant (in host context)
                using (_currentTenant.Change(null)) // Ensure host context
                {
                    // Check if tenant already exists
                    var existingTenant = await _tenantRepository.FindByNameAsync(tenantName);
                    if (existingTenant != null)
                    {
                        tenantName = $"{tenantName}-{DateTime.UtcNow:HHmmss}";
                        _logger.LogWarning("TenantCreationAgent: Tenant name {OriginalName} exists, using: {TenantName}", 
                            tenantName, tenantName);
                    }

                    // Create tenant using ABP TenantManager
                    tenant = await _tenantManager.CreateAsync(tenantName);
                    
                    // Store onboarding state in ExtraProperties (ABP pattern)
                    tenant.ExtraProperties["OnboardingStatus"] = "Pending";
                    tenant.ExtraProperties["CreatedByAgent"] = "true";
                    tenant.ExtraProperties["CreatedAt"] = DateTime.UtcNow.ToString("O");
                    
                    await _tenantRepository.InsertAsync(tenant);
                    
                    _logger.LogInformation("TenantCreationAgent: Tenant created - Name={TenantName}, Id={TenantId}", 
                        tenantName, tenant.Id);
                }

                // Step 2: Create Admin User (in tenant context)
                using (_currentTenant.Change(tenant.Id)) // Switch to tenant context
                {
                    // Check if user already exists
                    var existingUser = await _userManager.FindByEmailAsync(adminEmail);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("TenantCreationAgent: User with email {Email} already exists, rolling back tenant creation", adminEmail);
                        await RollbackTenantCreationAsync(tenant, "User with email already exists");
                        throw new InvalidOperationException($"User with email {adminEmail} already exists");
                    }

                    // Create user using ABP IdentityUserManager
                    user = new AbpIdentityUser(
                        id: Guid.NewGuid(),
                        userName: adminEmail,
                        email: adminEmail,
                        tenantId: tenant.Id
                    );

                    user.SetEmailConfirmed(false); // Require email confirmation

                    // Create user with password
                    var createResult = await _userManager.CreateAsync(user, adminPassword);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                        _logger.LogWarning("TenantCreationAgent: User creation failed - {Errors}, rolling back tenant creation", errors);
                        await RollbackTenantCreationAsync(tenant, $"User creation failed: {errors}");
                        throw new InvalidOperationException($"User creation failed: {errors}");
                    }

                    _logger.LogInformation("TenantCreationAgent: Admin user created - Email={Email}, UserId={UserId}", adminEmail, user.Id);

                    // Ensure TenantAdmin role exists
                    AbpIdentityRole? adminRole = null;
                    try
                    {
                        adminRole = await _roleManager.FindByNameAsync("TenantAdmin");
                        if (adminRole == null)
                        {
                            adminRole = new AbpIdentityRole(
                                Guid.NewGuid(),
                                "TenantAdmin",
                                tenant.Id
                            );
                            var roleResult = await _roleManager.CreateAsync(adminRole);
                            if (!roleResult.Succeeded)
                            {
                                var roleErrors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                                _logger.LogWarning("TenantCreationAgent: Role creation failed - {Errors}, rolling back", roleErrors);
                                await RollbackTenantAndUserCreationAsync(tenant, user, $"Role creation failed: {roleErrors}");
                                throw new InvalidOperationException($"Role creation failed: {roleErrors}");
                            }
                            _logger.LogInformation("TenantCreationAgent: TenantAdmin role created for tenant {TenantId}", tenant.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "TenantCreationAgent: Error creating or finding TenantAdmin role, rolling back");
                        await RollbackTenantAndUserCreationAsync(tenant, user, $"Role error: {ex.Message}");
                        throw;
                    }

                    // Assign TenantAdmin role
                    try
                    {
                        var roleAssignResult = await _userManager.AddToRoleAsync(user, "TenantAdmin");
                        if (!roleAssignResult.Succeeded)
                        {
                            var roleErrors = string.Join("; ", roleAssignResult.Errors.Select(e => e.Description));
                            _logger.LogWarning("TenantCreationAgent: Role assignment failed - {Errors}, rolling back", roleErrors);
                            await RollbackTenantAndUserCreationAsync(tenant, user, $"Role assignment failed: {roleErrors}");
                            throw new InvalidOperationException($"Role assignment failed: {roleErrors}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "TenantCreationAgent: Error assigning TenantAdmin role, rolling back");
                        await RollbackTenantAndUserCreationAsync(tenant, user, $"Role assignment error: {ex.Message}");
                        throw;
                    }

                    // Track first admin user ID in tenant ExtraProperties
                    try
                    {
                        using (_currentTenant.Change(null)) // Switch back to host context to update tenant
                        {
                            tenant = await _tenantRepository.GetAsync(tenant.Id);
                            tenant.ExtraProperties["FirstAdminUserId"] = user.Id.ToString();
                            await _tenantRepository.UpdateAsync(tenant);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't rollback - tenant and user are created successfully
                        // This is a non-critical operation (metadata tracking)
                        _logger.LogWarning(ex, "TenantCreationAgent: Error updating tenant ExtraProperties with FirstAdminUserId. Tenant and user created successfully, but metadata update failed.");
                        // Continue - tenant and user are still valid
                    }
                }

                _logger.LogInformation("TenantCreationAgent: Tenant {TenantName} with admin {Email} created successfully. TenantId={TenantId}", 
                    tenantName, adminEmail, tenant.Id);

                return tenant.Id;
            }
            catch (InvalidOperationException)
            {
                // Re-throw InvalidOperationException (these are expected business logic errors)
                throw;
            }
            catch (Exception ex)
            {
                // Unexpected error - attempt rollback
                _logger.LogError(ex, "TenantCreationAgent: Unexpected error during tenant creation, attempting rollback");
                
                if (tenant != null)
                {
                    try
                    {
                        if (user != null)
                        {
                            await RollbackTenantAndUserCreationAsync(tenant, user, $"Unexpected error: {ex.Message}");
                        }
                        else
                        {
                            await RollbackTenantCreationAsync(tenant, $"Unexpected error: {ex.Message}");
                        }
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx, "TenantCreationAgent: CRITICAL - Rollback failed for tenant {TenantId}. Manual cleanup may be required.", tenant.Id);
                        // Don't throw - original exception is more important
                    }
                }
                
                throw new InvalidOperationException($"Tenant creation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Rollback tenant creation by deleting the tenant
        /// </summary>
        private async Task RollbackTenantCreationAsync(Tenant tenant, string reason)
        {
            try
            {
                using (_currentTenant.Change(null))
                {
                    _logger.LogWarning("TenantCreationAgent: Rolling back tenant creation - TenantId={TenantId}, Reason={Reason}", 
                        tenant.Id, reason);
                    await _tenantRepository.DeleteAsync(tenant);
                    _logger.LogInformation("TenantCreationAgent: Tenant rollback completed - TenantId={TenantId}", tenant.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TenantCreationAgent: ERROR - Failed to rollback tenant creation - TenantId={TenantId}. Manual cleanup required.", tenant.Id);
                // Don't throw - log error for manual cleanup
            }
        }

        /// <summary>
        /// Rollback tenant and user creation by deleting both
        /// </summary>
        private async Task RollbackTenantAndUserCreationAsync(Tenant tenant, AbpIdentityUser user, string reason)
        {
            try
            {
                // First, try to delete the user (in tenant context)
                try
                {
                    using (_currentTenant.Change(tenant.Id))
                    {
                        _logger.LogWarning("TenantCreationAgent: Rolling back user creation - UserId={UserId}, Reason={Reason}", 
                            user.Id, reason);
                        var deleteResult = await _userManager.DeleteAsync(user);
                        if (!deleteResult.Succeeded)
                        {
                            var errors = string.Join("; ", deleteResult.Errors.Select(e => e.Description));
                            _logger.LogWarning("TenantCreationAgent: User deletion had errors (may have been partially created) - {Errors}", errors);
                        }
                        else
                        {
                            _logger.LogInformation("TenantCreationAgent: User rollback completed - UserId={UserId}", user.Id);
                        }
                    }
                }
                catch (Exception userEx)
                {
                    _logger.LogWarning(userEx, "TenantCreationAgent: Error during user rollback (user may not exist). Continuing with tenant rollback.");
                }

                // Then, delete the tenant (in host context)
                await RollbackTenantCreationAsync(tenant, reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TenantCreationAgent: ERROR - Failed to rollback tenant and user creation - TenantId={TenantId}, UserId={UserId}. Manual cleanup required.", 
                    tenant.Id, user.Id);
                // Don't throw - log error for manual cleanup
            }
        }
    }
}
