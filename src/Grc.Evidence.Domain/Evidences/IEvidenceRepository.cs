using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Enums;
using Volo.Abp.Domain.Repositories;

namespace Grc.Evidence;

/// <summary>
/// Repository interface for Evidence entity
/// </summary>
public interface IEvidenceRepository : IRepository<Evidence, Guid>
{
    Task<List<Evidence>> GetByControlAssessmentAsync(Guid controlAssessmentId);
    Task<List<Evidence>> GetByTypeAsync(EvidenceType evidenceType);
    Task<List<Evidence>> GetByUploaderAsync(Guid userId);
    Task<Evidence> GetCurrentVersionAsync(Guid evidenceId);
    Task<List<Evidence>> GetVersionsAsync(Guid evidenceId);
}

