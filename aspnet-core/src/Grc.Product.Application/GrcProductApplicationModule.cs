using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using Grc.Product.Application.Contracts;

namespace Grc.Product;

[DependsOn(
    typeof(GrcProductDomainModule),
    typeof(GrcProductApplicationContractsModule),
    typeof(AbpAutoMapperModule)
)]
public class GrcProductApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<GrcProductApplicationModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<GrcProductApplicationModule>(validate: true);
        });
    }
}

