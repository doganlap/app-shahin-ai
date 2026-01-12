using Volo.Abp.Modularity;

namespace Grc;

/* Inherit from this class for your domain layer tests. */
public abstract class GrcDomainTestBase<TStartupModule> : GrcTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
