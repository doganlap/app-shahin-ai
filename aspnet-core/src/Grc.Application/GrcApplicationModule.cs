using System;
using Grc.Integration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Grc.FrameworkLibrary.Application;

namespace Grc;

[DependsOn(
    typeof(GrcDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(GrcApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(FrameworkLibraryApplicationModule)
    )]
public class GrcApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        // Register services from Evidence, Risk, and FrameworkLibrary modules
        context.Services.AddAssemblyOf<Grc.Evidence.Application.Evidences.EvidenceAppService>();
        context.Services.AddAssemblyOf<Grc.Risk.Application.Risks.RiskAppService>();
        context.Services.AddAssemblyOf<Grc.FrameworkLibrary.Application.Frameworks.FrameworkAppService>();

        // Register HTTP clients for n8n and Ollama integration
        context.Services.AddHttpClient("N8n", client =>
        {
            var n8nUrl = configuration["N8n:Url"] ?? "http://localhost:5678";
            client.BaseAddress = new Uri(n8nUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        context.Services.AddHttpClient("Ollama", client =>
        {
            var ollamaUrl = configuration["Ollama:Url"] ?? "http://localhost:11434";
            client.BaseAddress = new Uri(ollamaUrl);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        // Register AI integration services
        context.Services.AddTransient<IN8nClientService, N8nClientService>();
        context.Services.AddTransient<IAiOrchestrationService, AiOrchestrationService>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<GrcApplicationModule>();
        });
    }
}
