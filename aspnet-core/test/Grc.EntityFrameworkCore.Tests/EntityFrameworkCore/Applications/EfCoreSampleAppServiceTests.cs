using Grc.Samples;
using Xunit;

namespace Grc.EntityFrameworkCore.Applications;

[Collection(GrcTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<GrcEntityFrameworkCoreTestModule>
{

}
