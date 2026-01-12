using Grc.FrameworkLibrary.Application.Contracts;
using Grc.FrameworkLibrary.Domain;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Grc.FrameworkLibrary.Application;

[DependsOn(
    typeof(FrameworkLibraryDomainModule),
    typeof(FrameworkLibraryApplicationContractsModule),
    typeof(AbpAutoMapperModule)
)]
public class FrameworkLibraryApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<FrameworkLibraryApplicationModule>();
        });
    }
}

