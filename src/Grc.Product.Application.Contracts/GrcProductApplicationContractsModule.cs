using Volo.Abp.Application.Contracts;
using Volo.Abp.Modularity;

namespace Grc.Product;

[DependsOn(
    typeof(AbpDddApplicationContractsModule)
)]
public class GrcProductApplicationContractsModule : AbpModule
{
}

