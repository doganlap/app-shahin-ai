using Volo.Abp.Modularity;

namespace Grc;

[DependsOn(
    typeof(GrcApplicationModule),
    typeof(GrcDomainTestModule)
)]
public class GrcApplicationTestModule : AbpModule
{

}
