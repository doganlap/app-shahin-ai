using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Grc.Assessment;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(GrcDomainSharedModule)
)]
public class GrcAssessmentDomainModule : AbpModule
{
}
