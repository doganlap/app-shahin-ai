using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Grc.Product;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpMultiTenancyModule)
)]
public class GrcProductDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Domain services are automatically registered by ABP convention
    }
}

