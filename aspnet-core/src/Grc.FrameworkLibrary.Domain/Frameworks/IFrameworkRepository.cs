using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Enums;
using Volo.Abp.Domain.Repositories;

namespace Grc.FrameworkLibrary.Domain.Frameworks;

/// <summary>
/// Repository interface for Framework entity
/// </summary>
public interface IFrameworkRepository : IRepository<Framework, Guid>
{
    Task<Framework> GetWithControlsAsync(Guid id);
    Task<Framework> GetByCodeAndVersionAsync(string code, string version);
    Task<List<Framework>> GetListByRegulatorAsync(Guid regulatorId);
    Task<List<Framework>> GetActiveFrameworksAsync();
    Task<List<Framework>> GetMandatoryFrameworksAsync();
    Task<List<Framework>> GetFrameworksByCategoryAsync(FrameworkCategory category);
    Task<bool> ExistsByCodeAndVersionAsync(string code, string version);
}

