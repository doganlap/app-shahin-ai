using Volo.Abp.Modularity;

namespace Grc;

[DependsOn(
    typeof(GrcDomainModule),
    typeof(GrcTestBaseModule)
)]
public class GrcDomainTestModule : AbpModule
{

}
