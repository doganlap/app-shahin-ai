using GrcMvc.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using GrcMvc.Data;
using GrcMvc.Models.Entities;
using AbpTenant = Volo.Abp.TenantManagement.Tenant;
using AppTenant = GrcMvc.Models.Entities.Tenant;

namespace GrcMvc.Data.Seeds;

/// <summary>
/// Seeds 5 trial tenants with admin users for onboarding testing
/// Each tenant represents a different industry/scenario
/// </summary>
public static class TrialTenantSeeds
{
    public static async Task SeedTrialTenantsAsync(
        GrcDbContext context,
        Volo.Abp.TenantManagement.ITenantManager tenantManager,
        Volo.Abp.Identity.IdentityUserManager userManager,
        Volo.Abp.Identity.IdentityRoleManager roleManager,
        ILogger logger)
    {
        logger.LogInformation("🌱 Starting trial tenant seeding...");

        var trialTenants = new[]
        {
            new { 
                Name = "AlRajhi Bank", 
                Slug = "alrajhi-bank",
                Email = "admin@alrajhi-demo.sa",
                Sector = "Banking",
                Country = "SA",
                Password = "AlRajhi@2026Demo"
            },
            new { 
                Name = "Saudi Telecom Company", 
                Slug = "stc-telecom",
                Email = "admin@stc-demo.sa",
                Sector = "Telecom",
                Country = "SA",
                Password = "STC@2026Demo"
            },
            new { 
                Name = "SABIC Industries", 
                Slug = "sabic-industries",
                Email = "admin@sabic-demo.sa",
                Sector = "Energy",
                Country = "SA",
                Password = "SABIC@2026Demo"
            },
            new { 
                Name = "King Faisal Hospital", 
                Slug = "kfh-hospital",
                Email = "admin@kfh-demo.sa",
                Sector = "Healthcare",
                Country = "SA",
                Password = "KFH@2026Demo"
            },
            new { 
                Name = "Noon E-Commerce", 
                Slug = "noon-ecommerce",
                Email = "admin@noon-demo.ae",
                Sector = "Retail",
                Country = "AE",
                Password = "Noon@2026Demo"
            }
        };

        foreach (var trial in trialTenants)
        {
            try
            {
                // Check if tenant already exists
                var existingTenant = await context.Set<AbpTenant>().FirstOrDefaultAsync(t => t.Name == trial.Slug);
                if (existingTenant != null)
                {
                    logger.LogInformation($"✓ Tenant '{trial.Name}' already exists, skipping...");
                    continue;
                }

                // Create ABP Tenant
                logger.LogInformation($"Creating tenant: {trial.Name}...");
                var tenant = await tenantManager.CreateAsync(trial.Slug);
                await context.SaveChangesAsync();

                // Create application Tenant record
                var appTenant = new GrcMvc.Models.Entities.Tenant
                {
                    Id = tenant.Id,
                    TenantSlug = trial.Slug,
                    OrganizationName = trial.Name,
                    AdminEmail = trial.Email,
                    Status = "Active",
                    SubscriptionTier = "Professional",
                    ActivatedAt = DateTime.UtcNow,
                    ActivationToken = string.Empty,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                context.Tenants.Add(appTenant);
                await context.SaveChangesAsync();

                // Create admin user for this tenant
                logger.LogInformation($"Creating admin user for {trial.Name}...");
                
                var adminUser = new Volo.Abp.Identity.IdentityUser(
                    Guid.NewGuid(),
                    trial.Email,
                    trial.Email,
                    tenant.Id
                )
                {
                    Name = "Admin",
                    Surname = trial.Name.Split(' ')[0]
                };
                
                adminUser.SetEmailConfirmed(true);
                adminUser.SetIsActive(true);

                var createResult = await userManager.CreateAsync(adminUser, trial.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    logger.LogError($"Failed to create user for {trial.Name}: {errors}");
                    continue;
                }

                // Assign admin role
                var adminRole = await roleManager.FindByNameAsync("admin");
                if (adminRole != null)
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }

                // Create TenantUser link
                var tenantUser = new TenantUser
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant.Id,
                    UserId = adminUser.Id.ToString(),
                    RoleCode = "TenantAdmin",
                    TitleCode = "Administrator",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                context.TenantUsers.Add(tenantUser);
                await context.SaveChangesAsync();

                // Create incomplete OnboardingWizard to trigger onboarding flow
                var wizard = new OnboardingWizard
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant.Id,
                    OrganizationLegalNameEn = trial.Name,
                    CountryOfIncorporation = trial.Country,
                    IndustrySector = trial.Sector,
                    PrimaryLanguage = "bilingual",
                    OrganizationType = "enterprise",
                    CurrentStep = 1,
                    IsCompleted = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = trial.Email
                };
                context.OnboardingWizards.Add(wizard);
                await context.SaveChangesAsync();

                logger.LogInformation($"✅ Created tenant '{trial.Name}' with admin user {trial.Email}");
                logger.LogInformation($"   Password: {trial.Password}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"❌ Error creating tenant '{trial.Name}'");
            }
        }

        logger.LogInformation("✅ Trial tenant seeding completed.");
    }
}
