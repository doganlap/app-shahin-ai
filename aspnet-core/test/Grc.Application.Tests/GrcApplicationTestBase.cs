using Volo.Abp.Modularity;

namespace Grc;

public abstract class GrcApplicationTestBase<TStartupModule> : GrcTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
