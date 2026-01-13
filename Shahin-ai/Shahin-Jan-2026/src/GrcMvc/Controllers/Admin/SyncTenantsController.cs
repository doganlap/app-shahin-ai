using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;
using GrcMvc.Data;
using GrcMvc.Scripts;

namespace GrcMvc.Controllers.Admin;

/// <summary>
/// Admin controller to sync existing tenants from custom Tenants table to ABP AbpTenants table
/// </summary>
[Authorize(Roles = "Admin")]
[Route("admin/sync-tenants")]
public class SyncTenantsController : Controller
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SyncTenantsController> _logger;
    private readonly GrcDbContext _dbContext;

    public SyncTenantsController(
        IServiceProvider serviceProvider,
        ILogger<SyncTenantsController> logger,
        GrcDbContext dbContext)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// GET: /admin/sync-tenants
    /// Shows sync status and allows manual sync
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var customTenants = await _dbContext.Tenants
            .Select(t => new
            {
                t.Id,
                t.TenantSlug,
                t.OrganizationName,
                t.Email,
                t.IsTrial,
                t.Status
            })
            .ToListAsync();

        var abpTenants = await _dbContext.Set<Volo.Abp.TenantManagement.Tenant>()
            .Select(t => new
            {
                t.Id,
                t.Name
            })
            .ToListAsync();

        var syncStatus = customTenants.Select(ct => new
        {
            CustomTenant = ct,
            IsSynced = abpTenants.Any(abp => abp.Id == ct.Id),
            AbpTenant = abpTenants.FirstOrDefault(abp => abp.Id == ct.Id)
        }).ToList();

        ViewBag.SyncStatus = syncStatus;
        ViewBag.TotalCustom = customTenants.Count;
        ViewBag.TotalAbp = abpTenants.Count;
        ViewBag.Synced = syncStatus.Count(s => s.IsSynced);
        ViewBag.NotSynced = syncStatus.Count(s => !s.IsSynced);

        return View();
    }

    /// <summary>
    /// POST: /admin/sync-tenants/sync
    /// Executes the sync operation
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await SyncExistingTenantsToAbp.RunAsync(_serviceProvider);
            TempData["SuccessMessage"] = "Tenant sync completed successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during tenant sync");
            TempData["ErrorMessage"] = $"Error during sync: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
