using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Grc.FrameworkLibrary.Application.Contracts.Regulators;

public interface IRegulatorAppService : ICrudAppService<
    RegulatorDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateRegulatorDto,
    CreateUpdateRegulatorDto>
{
}

