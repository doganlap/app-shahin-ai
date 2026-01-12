using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Grc.FrameworkLibrary.Regulators;

/// <summary>
/// Repository interface for Regulator entity
/// </summary>
public interface IRegulatorRepository : IRepository<Regulator, Guid>
{
    Task<Regulator> GetByCodeAsync(string code);
    Task<bool> ExistsByCodeAsync(string code);
}

