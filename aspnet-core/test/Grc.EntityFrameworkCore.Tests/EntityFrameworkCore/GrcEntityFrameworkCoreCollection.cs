using Xunit;

namespace Grc.EntityFrameworkCore;

[CollectionDefinition(GrcTestConsts.CollectionDefinitionName)]
public class GrcEntityFrameworkCoreCollection : ICollectionFixture<GrcEntityFrameworkCoreFixture>
{

}
