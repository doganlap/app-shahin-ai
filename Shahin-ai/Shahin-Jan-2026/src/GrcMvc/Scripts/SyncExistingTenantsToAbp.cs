using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;
using GrcMvc.Data;
using GrcMvc.Models.Entities;

namespace GrcMvc.Scripts;

/// <summary>
/// Script to sync existing tenants from custom Tenants table to ABP AbpTenants table
/// Run this once to migrate existing tenant data to ABP Framework
/// </summary>
public class SyncExistingTenantsToAbp
{
    public static async Task RunAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<SyncExistingTenantsToAbp>>();
        var dbContext = serviceProvider.GetRequiredService<GrcDbContext>();
        var tenantManager = serviceProvider.GetRequiredService<ITenantManager>();
        var tenantRepository = serviceProvider.GetRequiredService<ITenantRepository>();
        var currentTenant = serviceProvider.GetRequiredService<ICurrentTenant>();

        logger.LogInformation("Starting tenant sync from custom Tenants table to ABP AbpTenants table...");

        // Get all tenants from custom table that don't exist in ABP
        // Use host context to query all tenants
        using (currentTenant.Change(null))
        {
            var abpTenantIds = await tenantRepository.GetListAsync();
            var existingAbpIds = abpTenantIds.Select(t => t.Id).ToHashSet();

            var customTenants = await dbContext.Tenants
                .Where(t => !existingAbpIds.Contains(t.Id) && !t.IsDeleted)
                .ToListAsync();

            if (customTenants.Count == 0)
            {
                logger.LogInformation("No tenants to sync. All tenants are already in ABP.");
                return;
            }

            logger.LogInformation("Found {Count} tenants to sync", customTenants.Count);

            int successCount = 0;
            int errorCount = 0;

            foreach (var customTenant in customTenants)
            {
                try
                {
                    // Check if ABP tenant already exists by name
                    var existingAbpTenant = await tenantRepository.FindByNameAsync(customTenant.TenantSlug);
                    if (existingAbpTenant != null)
                    {
                        logger.LogWarning("ABP tenant already exists with name '{TenantName}' (ID: {ExistingId}), skipping custom tenant {CustomId}",
                            customTenant.TenantSlug, existingAbpTenant.Id, customTenant.Id);
                        continue;
                    }

                    // Create ABP tenant (ABP will generate the ID)
                    var abpTenant = await tenantManager.CreateAsync(customTenant.TenantSlug);
                    
                    // Note: ABP generates its own ID. If you need to maintain the same ID,
                    // you'll need to update the custom Tenant record to reference the new ABP tenant ID
                    // or handle ID mapping separately to maintain referential integrity

                    // Insert into repository
                    await tenantRepository.InsertAsync(abpTenant);
                    await dbContext.SaveChangesAsync();
                    
                    // Optionally update custom tenant to reference ABP tenant ID if needed
                    // customTenant.Id = abpTenant.Id;
                    // await dbContext.SaveChangesAsync();

                    logger.LogInformation("✅ Synced tenant: {TenantName} (ID: {TenantId})",
                        customTenant.TenantSlug, customTenant.Id);
                    successCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Failed to sync tenant {TenantName} (ID: {TenantId}): {Error}",
                        customTenant.TenantSlug, customTenant.Id, ex.Message);
                    errorCount++;
                }
            }

            logger.LogInformation("Tenant sync completed. Success: {Success}, Errors: {Errors}",
                successCount, errorCount);
        }
    }
}
