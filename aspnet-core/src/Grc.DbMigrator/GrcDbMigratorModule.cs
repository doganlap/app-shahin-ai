using Grc.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Grc.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(GrcEntityFrameworkCoreModule),
    typeof(GrcApplicationContractsModule)
    )]
public class GrcDbMigratorModule : AbpModule
{
}
