using GrcMvc.Data;
using GrcMvc.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;

namespace GrcMvc.Services.Implementations;

public sealed class TenantCreationService : ITenantCreationService
{
    private readonly GrcDbContext _db;
    private readonly ITenantManager _tenantManager;
    private readonly ITenantRepository _tenantRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<TenantCreationService> _logger;

    public TenantCreationService(
        GrcDbContext db,
        ITenantManager tenantManager,
        ITenantRepository tenantRepository,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        ICurrentTenant currentTenant,
        ILogger<TenantCreationService> logger)
    {
        _db = db;
        _tenantManager = tenantManager;
        _tenantRepository = tenantRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _currentTenant = currentTenant;
        _logger = logger;
    }

    public async Task<TenantCreationResult> CreateTenantWithAdminAsync(
        TenantCreationRequest req,
        ClaimsPrincipal actor)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(req.TenantName) ||
            string.IsNullOrWhiteSpace(req.AdminEmail))
        {
            return new TenantCreationResult 
            { 
                Success = false, 
                Error = "Tenant name and admin email are required." 
            };
        }

        var tenantName = req.TenantName.Trim().ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace(",", "");
        var email = req.AdminEmail.Trim().ToLowerInvariant();

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            // Check if tenant exists
            var existingTenant = await _tenantRepository.FindByNameAsync(tenantName);
            if (existingTenant != null)
            {
                return new TenantCreationResult 
                { 
                    Success = false, 
                    Error = $"Tenant '{tenantName}' already exists." 
                };
            }

            // Create tenant (in host context)
            Volo.Abp.TenantManagement.Tenant tenant;
            using (_currentTenant.Change(null))
            {
                tenant = await _tenantManager.CreateAsync(tenantName);
                await _tenantRepository.InsertAsync(tenant);
                await _db.SaveChangesAsync();
            }

            _logger.LogInformation("Created tenant: {TenantName} (ID: {TenantId})", tenantName, tenant.Id);

            // Create admin user (in tenant context)
            Volo.Abp.Identity.IdentityUser user;
            using (_currentTenant.Change(tenant.Id))
            {
                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    await tx.RollbackAsync();
                    return new TenantCreationResult 
                    { 
                        Success = false, 
                        Error = $"User with email '{email}' already exists." 
                    };
                }

                user = new Volo.Abp.Identity.IdentityUser(
                    id: Guid.NewGuid(),
                    userName: email,
                    email: email,
                    tenantId: tenant.Id
                );

                user.SetEmailConfirmed(req.EmailConfirmed);

                // Generate password if not provided
                var password = req.AdminPassword;
                if (string.IsNullOrWhiteSpace(password))
                {
                    password = Guid.NewGuid().ToString("N") + "!Aa1";
                }

                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    await tx.RollbackAsync();
                    return new TenantCreationResult 
                    { 
                        Success = false, 
                        Error = $"User creation failed: {errors}" 
                    };
                }

                // Ensure TenantAdmin role exists
                var adminRole = await _roleManager.FindByNameAsync("TenantAdmin");
                if (adminRole == null)
                {
                    adminRole = new Volo.Abp.Identity.IdentityRole(
                        Guid.NewGuid(), 
                        "TenantAdmin", 
                        tenant.Id
                    );
                    await _roleManager.CreateAsync(adminRole);
                }

                // Assign role
                var roleResult = await _userManager.AddToRoleAsync(user, "TenantAdmin");
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    await tx.RollbackAsync();
                    return new TenantCreationResult 
                    { 
                        Success = false, 
                        Error = $"Role assignment failed: {errors}" 
                    };
                }

                await _db.SaveChangesAsync();
            }

            _logger.LogInformation(
                "Created admin user {Email} for tenant {TenantName} by {Actor}",
                email, tenantName, actor.Identity?.Name ?? "System");

            await tx.CommitAsync();

            return new TenantCreationResult
            {
                Success = true,
                TenantId = tenant.Id,
                AdminUserId = user.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tenant creation failed for {TenantName}", req.TenantName);
            await tx.RollbackAsync();
            return new TenantCreationResult 
            { 
                Success = false, 
                Error = $"Tenant creation failed: {ex.Message}" 
            };
        }
    }
}
