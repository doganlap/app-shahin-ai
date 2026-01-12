using System.Threading.Tasks;
using Grc.FrameworkLibrary.Domain.Data;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Grc.FrameworkLibrary.Domain.Seeders;

public class RegulatorDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly RegulatorDataSeeder _regulatorDataSeeder;

    public RegulatorDataSeedContributor(RegulatorDataSeeder regulatorDataSeeder)
    {
        _regulatorDataSeeder = regulatorDataSeeder;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await _regulatorDataSeeder.SeedAsync();
    }
}

