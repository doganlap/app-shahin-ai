using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.FrameworkLibrary.Frameworks;
using Volo.Abp.Application.Services;

namespace Grc.FrameworkLibrary.Application.Contracts.Frameworks;

/// <summary>
/// Application service interface for Framework operations
/// </summary>
public interface IFrameworkAppService : IApplicationService
{
    Task<PagedResultDto<FrameworkDto>> GetListAsync(GetFrameworkListInput input);
    Task<FrameworkDto> GetAsync(Guid id);
    Task<PagedResultDto<ControlDto>> GetControlsAsync(Guid frameworkId, GetControlListInput input);
    Task<List<ControlDto>> SearchControlsAsync(string query, List<Guid> frameworkIds = null);
}

