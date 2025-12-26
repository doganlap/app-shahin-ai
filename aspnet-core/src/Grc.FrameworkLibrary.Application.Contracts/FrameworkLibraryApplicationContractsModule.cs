using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Grc.FrameworkLibrary.Application.Contracts;

[DependsOn(
    typeof(AbpDddApplicationContractsModule)
)]
public class FrameworkLibraryApplicationContractsModule : AbpModule
{
}

