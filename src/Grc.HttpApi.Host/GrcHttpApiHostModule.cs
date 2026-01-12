using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.AspNetCore.SignalR;

namespace Grc;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreSignalRModule)
)]
public class GrcHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = context.Services.GetConfiguration();

        // Configure Multi-Tenancy with domain-based resolution
        Configure<AbpTenantResolveOptions>(options =>
        {
            options.TenantResolvers.Clear();
            // Domain-based tenant resolution: {tenant}.grc-platform.sa
            options.TenantResolvers.Add(new DomainTenantResolveContributor("{0}.grc-platform.sa"));
            // Fallback to header-based resolution
            options.TenantResolvers.Add(new HeaderTenantResolveContributor());
            // Fallback to cookie-based resolution
            options.TenantResolvers.Add(new CookieTenantResolveContributor());
        });

        // Configure CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        "https://*.grc-platform.sa",
                        "http://localhost:4200",
                        "http://localhost:3000"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAbpRequestLocalization();
        app.UseConfiguredEndpoints();
    }
}

