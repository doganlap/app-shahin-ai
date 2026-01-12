using System;
using System.Linq;
using System.Threading.Tasks;
using Grc.FrameworkLibrary.Application.Contracts.Regulators;
using Grc.FrameworkLibrary.Domain.Regulators;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.FrameworkLibrary.Application.Regulators;

public class RegulatorAppService : CrudAppService<
    Regulator,
    RegulatorDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateRegulatorDto,
    CreateUpdateRegulatorDto>,
    IRegulatorAppService
{
    public RegulatorAppService(IRepository<Regulator, Guid> repository)
        : base(repository)
    {
    }

    public override async Task<PagedResultDto<RegulatorDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await Repository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        
        var regulators = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(r => r.Code)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        var dtos = ObjectMapper.Map<System.Collections.Generic.List<Regulator>, System.Collections.Generic.List<RegulatorDto>>(regulators);

        // Log to systemd journal instead
        Console.WriteLine($"[DEBUG] RegulatorAppService: totalCount={totalCount}, dtoCount={dtos.Count}, firstCode={regulators.FirstOrDefault()?.Code ?? "null"}");

        return new PagedResultDto<RegulatorDto>(totalCount, dtos);
    }
}

