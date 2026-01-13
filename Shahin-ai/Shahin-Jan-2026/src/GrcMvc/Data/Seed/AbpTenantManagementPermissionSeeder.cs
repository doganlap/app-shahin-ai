using System;
using System.Linq;
using System.Threading.Tasks;
using GrcMvc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace GrcMvc.Data.Seed
{
    /// <summary>
    /// Seeds ABP TenantManagement permissions and assigns them to host admin roles
    /// This enables access to /TenantManagement/Tenants UI
    /// </summary>
    public class AbpTenantManagementPermissionSeeder : ITransientDependency
    {
        private readonly IPermissionGrantRepository _permissionGrantRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IdentityRoleManager _roleManager;
        private readonly ILogger<AbpTenantManagementPermissionSeeder> _logger;

        public AbpTenantManagementPermissionSeeder(
            IPermissionGrantRepository permissionGrantRepository,
            ICurrentTenant currentTenant,
            IdentityRoleManager roleManager,
            ILogger<AbpTenantManagementPermissionSeeder> logger)
        {
            _permissionGrantRepository = permissionGrantRepository;
            _currentTenant = currentTenant;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Seeds ABP TenantManagement permissions and assigns to host admin roles
        /// </summary>
        [UnitOfWork]
        public async Task SeedAsync()
        {
            // #region agent log
            try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:43", message = "SeedAsync entry", data = new { repositoryExists = _permissionGrantRepository != null, roleManagerExists = _roleManager != null, currentTenantId = _currentTenant.Id?.ToString() ?? "null", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            try
            {
                _logger.LogInformation("Seeding ABP TenantManagement permissions...");

                // Ensure we're in host context (no tenant)
                using (_currentTenant.Change(null))
                {
                    // #region agent log
                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:50", message = "Host context established", data = new { tenantIdAfterChange = _currentTenant.Id?.ToString() ?? "null", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                    // ABP TenantManagement permission names (auto-registered by ABP framework)
                    var tenantManagementPermissions = new[]
                    {
                        "TenantManagement.Tenants",
                        "TenantManagement.Tenants.Create",
                        "TenantManagement.Tenants.Edit",
                        "TenantManagement.Tenants.Delete"
                    };

                    _logger.LogDebug("Granting {Count} TenantManagement permissions to host admin roles", tenantManagementPermissions.Length);

                    // Assign permissions to host admin roles
                    var hostAdminRoles = new[] { "PlatformAdmin", "Admin", "SuperAdmin" };

                    foreach (var roleName in hostAdminRoles)
                    {
                        // #region agent log
                        try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:66", message = "Processing role", data = new { roleName = roleName, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                        var role = await _roleManager.FindByNameAsync(roleName);
                        // #region agent log
                        try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:68", message = "Role lookup result", data = new { roleName = roleName, roleFound = role != null, roleId = role?.Id.ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                        if (role != null)
                        {
                            foreach (var permissionName in tenantManagementPermissions)
                            {
                                try
                                {
                                    // Check if permission grant already exists to avoid duplicates
                                    var allGrants = await _permissionGrantRepository.GetListAsync();
                                    // #region agent log
                                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:76", message = "Permission grants retrieved", data = new { totalGrants = allGrants.Count, permissionName = permissionName, roleName = roleName, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                                    // #endregion
                                    var existingGrant = allGrants.FirstOrDefault(g =>
                                        g.Name == permissionName &&
                                        g.ProviderName == "R" &&
                                        g.ProviderKey == roleName &&
                                        g.TenantId == null);

                                    if (existingGrant != null)
                                    {
                                        // #region agent log
                                        try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:84", message = "Duplicate grant found", data = new { permissionName = permissionName, roleName = roleName, existingGrantId = existingGrant.Id.ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                                        // #endregion
                                        _logger.LogDebug("Permission {Permission} already granted to role {Role}, skipping", permissionName, roleName);
                                        continue;
                                    }

                                    // Use ABP's permission grant repository to assign permission to role
                                    // ProviderName: "R" (for role-based permissions in ABP)
                                    // ProviderKey: Role name
                                    // TenantId: null (host level permissions)
                                    var grantId = Guid.NewGuid();
                                    // #region agent log
                                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:95", message = "Before permission grant insert", data = new { grantId = grantId.ToString(), permissionName = permissionName, roleName = roleName, providerName = "R", tenantId = "null", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                                    // #endregion
                                    await _permissionGrantRepository.InsertAsync(
                                        new PermissionGrant(
                                            grantId,
                                            permissionName,
                                            "R",  // ProviderName: "R" for Role provider (ABP standard)
                                            roleName,  // ProviderKey: Role name
                                            null  // TenantId: null for host
                                        ),
                                        autoSave: true
                                    );
                                    // #region agent log
                                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:103", message = "Permission grant inserted", data = new { grantId = grantId.ToString(), permissionName = permissionName, roleName = roleName, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                                    // #endregion
                                    _logger.LogDebug("Granted permission {Permission} to role {Role}", permissionName, roleName);
                                }
                                catch (Exception ex)
                                {
                                    // #region agent log
                                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:108", message = "Permission grant exception", data = new { exceptionType = ex.GetType().Name, exceptionMessage = ex.Message, permissionName = permissionName, roleName = roleName, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                                    // #endregion
                                    _logger.LogWarning(ex, "Failed to grant permission {Permission} to role {Role}", permissionName, roleName);
                                }
                            }
                            _logger.LogInformation("Assigned TenantManagement permissions to role: {RoleName}", roleName);
                        }
                        else
                        {
                            // #region agent log
                            try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:114", message = "Role not found", data = new { roleName = roleName, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                            // #endregion
                            _logger.LogDebug("Role not found: {RoleName} - skipping permission assignment", roleName);
                        }
                    }
                }

                _logger.LogInformation("✅ ABP TenantManagement permissions seeded successfully");
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:119", message = "SeedAsync exit success", data = new { timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            catch (Exception ex)
            {
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "C", location = "AbpTenantManagementPermissionSeeder.cs:122", message = "SeedAsync exception", data = new { exceptionType = ex.GetType().Name, exceptionMessage = ex.Message, stackTrace = ex.StackTrace?.Substring(0, Math.Min(500, (ex.StackTrace?.Length).GetValueOrDefault(0))), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                _logger.LogError(ex, "Error seeding ABP TenantManagement permissions");
                // Don't throw - allow application to continue
            }
        }
    }
}
