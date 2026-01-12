using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Grc.Product;

[DependsOn(
    typeof(AbpDddApplicationContractsModule)
)]
public class GrcProductApplicationContractsModule : AbpModule
{
}

