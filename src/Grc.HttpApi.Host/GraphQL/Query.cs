using System.Linq;
using System.Threading.Tasks;
using Grc.FrameworkLibrary.Application.Contracts.Frameworks;
using Grc.FrameworkLibrary.Frameworks;
using HotChocolate;
using HotChocolate.Data;
using Volo.Abp.Domain.Repositories;

namespace Grc.GraphQL;

/// <summary>
/// GraphQL Query root
/// </summary>
public class Query
{
    [UseDbContext]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Framework> GetFrameworks([ScopedService] IRepository<Framework, System.Guid> repository)
    {
        return repository.GetQueryableAsync().Result;
    }

    [UseDbContext]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Control> GetControls([ScopedService] IRepository<Control, System.Guid> repository)
    {
        return repository.GetQueryableAsync().Result;
    }

    public async Task<Framework> GetFramework(
        System.Guid id,
        [Service] IFrameworkAppService appService)
    {
        var dto = await appService.GetAsync(id);
        // TODO: Map DTO back to entity for GraphQL
        return null; // Placeholder
    }
}

