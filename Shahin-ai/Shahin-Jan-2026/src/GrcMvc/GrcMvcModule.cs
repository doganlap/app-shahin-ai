using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using GrcMvc.Data;

namespace GrcMvc
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpEntityFrameworkCorePostgreSqlModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpIdentityEntityFrameworkCoreModule),
        typeof(AbpIdentityAspNetCoreModule),
        typeof(AbpAccountWebModule),
        typeof(AbpAccountApplicationModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpTenantManagementApplicationContractsModule),
        typeof(AbpTenantManagementWebModule),
        typeof(AbpPermissionManagementDomainModule),
        typeof(AbpPermissionManagementEntityFrameworkCoreModule),
        typeof(AbpFeatureManagementDomainModule),
        typeof(AbpFeatureManagementEntityFrameworkCoreModule)
    )]
    public class GrcMvcModule : AbpModule
    {
        // Static constructor ensures Npgsql legacy behavior is set before any module loads
        static GrcMvcModule()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            // Also set here for redundancy
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
            // Get connection string from configuration (must be done in PreConfigureServices)
            // so it's available when UsePostgreSql() configures ABP module DbContexts
            var configuration = context.Services.GetConfiguration();
            var connectionString = configuration.GetConnectionString("Default") 
                ?? configuration.GetConnectionString("DefaultConnection");
            
            // Log connection string configuration (first 50 chars only for security)
            var connectionStringPreview = string.IsNullOrEmpty(connectionString) 
                ? "NOT SET" 
                : connectionString.Substring(0, Math.Min(50, connectionString.Length)) + "...";
            Console.WriteLine($"[GrcMvcModule] PreConfigureServices: Connection string configured (preview: {connectionStringPreview})");
            
            // Register connection string for ABP (must be done BEFORE UsePostgreSql)
            Configure<Volo.Abp.Data.AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = connectionString;
                Console.WriteLine($"[GrcMvcModule] PreConfigureServices: AbpDbConnectionOptions.Default set (preview: {connectionStringPreview})");
            });
            
            // CRITICAL: Configure ABP's PostgreSQL provider GLOBALLY using UsePostgreSql()
            // This ensures ALL ABP module DbContexts get the correct connection string
            PreConfigure<AbpDbContextOptions>(options =>
            {
                Console.WriteLine("[GrcMvcModule] PreConfigureServices: Configuring UsePostgreSql() for all ABP module DbContexts");
                // UsePostgreSql() is ABP's method to configure PostgreSQL for ALL DbContexts
                // It internally handles connection string resolution from AbpDbConnectionOptions
                options.UsePostgreSql(npgsqlOptions =>
                {
                    npgsqlOptions.CommandTimeout(60);
                    // CRITICAL: Do NOT call EnableRetryOnFailure()
                    // ABP UnitOfWork transactions require no retry strategy
                });
                Console.WriteLine("[GrcMvcModule] PreConfigureServices: UsePostgreSql() configured successfully");
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetHostingEnvironment();

            // Get connection string from configuration (ABP uses "Default" key)
            var connectionString = configuration.GetConnectionString("Default") 
                ?? configuration.GetConnectionString("DefaultConnection");
            
            // Register UTC DateTime interceptor as singleton for DI
            context.Services.AddSingleton<UtcDateTimeInterceptor>();
            
            // Configure EF Core DbContext for GrcMvc with connection string
            // This replaces ALL ABP module DbContexts with GrcDbContext
            context.Services.AddAbpDbContext<GrcDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
                
                // CRITICAL: Replace ABP's TenantManagement DbContext with GrcDbContext
                // This ensures ITenantRepository uses GrcDbContext (which is properly configured)
                // instead of ABP's TenantManagementDbContext (which may not be configured)
                options.ReplaceDbContext<Volo.Abp.TenantManagement.EntityFrameworkCore.ITenantManagementDbContext>();

                // CRITICAL: Replace ABP's Identity DbContext with GrcDbContext
                // This ensures ABP Identity tables (AbpUsers, AbpRoles, etc.) are managed by GrcDbContext
                // Required for ITenantAppService.CreateAsync() to work properly
                options.ReplaceDbContext<Volo.Abp.Identity.EntityFrameworkCore.IIdentityDbContext>();

                // CRITICAL: Replace ABP's PermissionManagement and FeatureManagement DbContexts with GrcDbContext
                // This ensures all ABP module entities use the same DbContext with proper connection string
                options.ReplaceDbContext<Volo.Abp.PermissionManagement.EntityFrameworkCore.IPermissionManagementDbContext>();
                options.ReplaceDbContext<Volo.Abp.FeatureManagement.EntityFrameworkCore.IFeatureManagementDbContext>();
            });
            
            // Register connection string for ABP
            Configure<Volo.Abp.Data.AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = connectionString;
            });

            // Configure Identity options
            Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 1;
            });

            // Configure ABP Multi-Tenancy
            Configure<AbpMultiTenancyOptions>(options =>
            {
                options.IsEnabled = true;
            });

            // Configure ABP Account self-registration
            // Note: AccountOptions configuration is typically done via appsettings.json
            // The Account module reads from "Account:SelfRegistration:IsEnabled" configuration

            // Explicitly configure ABP Account Web module controllers (optional but recommended)
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                // ABP modules auto-register controllers, but explicit config ensures clarity
                options.ConventionalControllers.Create(typeof(AbpAccountWebModule).Assembly);
            });

            // Configure ABP Basic Theme
            Configure<AbpThemingOptions>(options =>
            {
                // Use built-in BasicTheme instead of custom theme
                options.DefaultThemeName = BasicTheme.Name;
            });

            // CRITICAL: Configure ABP DbContext options with UTC DateTime interceptor
            // This ensures ALL ABP module DbContexts (Identity, Tenant, Permission, Feature)
            // convert DateTime values to UTC before saving to PostgreSQL timestamptz columns
            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(ctx =>
                {
                    var interceptor = ctx.ServiceProvider.GetRequiredService<UtcDateTimeInterceptor>();
                    ctx.DbContextOptions.AddInterceptors(interceptor);
                });
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            app.UseAbpRequestLocalization();
            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAbpClaimsMap();
            app.UseAuthorization();
            app.UseConfiguredEndpoints();
        }
    }
}
