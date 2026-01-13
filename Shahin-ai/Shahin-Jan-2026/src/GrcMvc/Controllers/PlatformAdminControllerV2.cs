using GrcMvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Volo.Abp.TenantManagement;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GrcMvc.Controllers;

/// <summary>
/// V2 Platform Admin Controller - Uses facade pattern for gradual migration
/// Routes: /platform-admin/v2/* (parallel to existing /platform-admin/*)
/// </summary>
[Route("platform-admin/v2")]
[Authorize(Roles = "PlatformAdmin")]
public class PlatformAdminControllerV2 : Controller
{
    private readonly IUserManagementFacade _userFacade;
    private readonly ILogger<PlatformAdminControllerV2> _logger;
    private readonly ITenantCreationService _tenantCreation;
    private readonly ITenantManager _tenantManager;
    private readonly ITenantRepository _tenantRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly ICurrentTenant _currentTenant;

    public PlatformAdminControllerV2(
        IUserManagementFacade userFacade,
        ILogger<PlatformAdminControllerV2> logger,
        ITenantCreationService tenantCreation,
        ITenantManager tenantManager,
        ITenantRepository tenantRepository,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        ICurrentTenant currentTenant)
    {
        _userFacade = userFacade;
        _logger = logger;
        _tenantCreation = tenantCreation;
        _tenantManager = tenantManager;
        _tenantRepository = tenantRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _currentTenant = currentTenant;
    }
    
    private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    
    /// <summary>
    /// V2 Dashboard (testing endpoint)
    /// </summary>
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        ViewData["Version"] = "V2 (Facade)";
        ViewData["Message"] = "This is the V2 controller using the facade pattern";
        return View("~/Views/PlatformAdmin/DashboardV2.cshtml");
    }
    
    /// <summary>
    /// Show manual tenant creation form
    /// </summary>
    [HttpGet("tenants/create")]
    public IActionResult CreateTenant()
    {
        return View(new CreateTenantModel());
    }
    
    /// <summary>
    /// Create tenant and admin user manually
    /// </summary>
    [HttpPost("tenants/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTenant(CreateTenantModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Create tenant
            var tenant = await _tenantManager.CreateAsync(model.TenantName);
            await _tenantRepository.InsertAsync(tenant);
            
            _logger.LogInformation("Platform admin created tenant: {TenantName} (ID: {TenantId})", model.TenantName, tenant.Id);

            // Create admin user in tenant context
            using (_currentTenant.Change(tenant.Id))
            {
                var user = new Volo.Abp.Identity.IdentityUser(
                    id: Guid.NewGuid(),
                    userName: model.AdminEmail,
                    email: model.AdminEmail,
                    tenantId: tenant.Id
                );

                user.SetEmailConfirmed(true); // Platform admin creates confirmed users
                
                var createResult = await _userManager.CreateAsync(user, model.AdminPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    ModelState.AddModelError("", $"User creation failed: {errors}");
                    
                    // Rollback tenant
                    await _tenantRepository.DeleteAsync(tenant);
                    return View(model);
                }

                // Create or get TenantAdmin role
                var adminRole = await _roleManager.FindByNameAsync("TenantAdmin");
                if (adminRole == null)
                {
                    adminRole = new Volo.Abp.Identity.IdentityRole(Guid.NewGuid(), "TenantAdmin", tenant.Id);
                    await _roleManager.CreateAsync(adminRole);
                }
                await _userManager.AddToRoleAsync(user, "TenantAdmin");
                
                _logger.LogInformation("Created admin user {Email} for tenant {TenantName}", model.AdminEmail, model.TenantName);
            }

            TempData["SuccessMessage"] = $"Tenant '{model.TenantName}' created successfully with admin user {model.AdminEmail}";
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant {TenantName}", model.TenantName);
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return View(model);
        }
    }
    
    /// <summary>
    /// Get user details (via facade)
    /// </summary>
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        try
        {
            var user = await _userFacade.GetUserAsync(id);
            return Json(new { success = true, data = user, source = "V2-Facade" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return Json(new { success = false, error = "An error occurred processing your request." });
        }
    }
    
    /// <summary>
    /// Reset user password (via facade)
    /// </summary>
    [HttpPost("users/{id}/reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string id)
    {
        try
        {
            var adminUserId = GetCurrentUserId();
            var newPassword = GenerateSecurePassword();
            
            var success = await _userFacade.ResetPasswordAsync(adminUserId, id, newPassword);
            
            if (success)
            {
                // Show password in secure modal (one-time display)
                ViewData["GeneratedPassword"] = newPassword;
                ViewData["ShowPasswordModal"] = true;
                TempData["Success"] = "Password reset successfully (V2)";
            }
            else
            {
                TempData["Error"] = "Failed to reset password";
            }
            
            return RedirectToAction("GetUser", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {UserId}", id);
            TempData["Error"] = "An error occurred. Please try again.";
            return RedirectToAction("GetUser", new { id });
        }
    }
    
    /// <summary>
    /// List all users (via facade)
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> Users(int page = 1, int pageSize = 50)
    {
        try
        {
            var users = await _userFacade.GetUsersAsync(page, pageSize);
            ViewData["Version"] = "V2 (Facade)";
            return View("~/Views/PlatformAdmin/UsersV2.cshtml", users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing users");
            TempData["Error"] = "An error occurred. Please try again.";
            return View("~/Views/PlatformAdmin/UsersV2.cshtml", new List<UserDto>());
        }
    }
    
    private static string GenerateSecurePassword()
    {
        const int length = 18;
        const string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%^&*";
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(length);
        var result = new char[length];
        
        for (int i = 0; i < length; i++)
            result[i] = validChars[bytes[i] % validChars.Length];
        
        return new string(result);
    }
}
