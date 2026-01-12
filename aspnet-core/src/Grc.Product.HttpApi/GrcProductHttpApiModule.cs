using Volo.Abp.Modularity;
using Grc.Product.Application.Contracts;

namespace Grc.Product.HttpApi;

[DependsOn(
    typeof(GrcProductApplicationContractsModule)
)]
public class GrcProductHttpApiModule : AbpModule
{
}

