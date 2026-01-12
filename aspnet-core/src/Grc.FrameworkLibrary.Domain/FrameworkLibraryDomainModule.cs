using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Grc.FrameworkLibrary.Domain;

[DependsOn(
    typeof(AbpDddDomainModule)
)]
public class FrameworkLibraryDomainModule : AbpModule
{
}

