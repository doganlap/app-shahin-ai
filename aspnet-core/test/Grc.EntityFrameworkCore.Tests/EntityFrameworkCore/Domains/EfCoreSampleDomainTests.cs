using Grc.Samples;
using Xunit;

namespace Grc.EntityFrameworkCore.Domains;

[Collection(GrcTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<GrcEntityFrameworkCoreTestModule>
{

}
