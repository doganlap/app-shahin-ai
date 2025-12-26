using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Enums;
using Volo.Abp.Domain.Repositories;

namespace Grc.FrameworkLibrary.Domain.Frameworks;

/// <summary>
/// Repository interface for Control entity
/// </summary>
public interface IControlRepository : IRepository<Control, Guid>
{
    Task<List<Control>> GetByFrameworkAsync(Guid frameworkId);
    Task<List<Control>> GetByDomainAsync(Guid frameworkId, string domainCode);
    Task<List<Control>> GetByTypeAsync(Guid frameworkId, ControlType type);
    Task<Control> GetByControlNumberAsync(Guid frameworkId, string controlNumber);
    Task<List<Control>> SearchAsync(string searchTerm, List<Guid> frameworkIds = null);
}

