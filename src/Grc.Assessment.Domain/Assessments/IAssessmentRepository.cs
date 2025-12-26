using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Enums;
using Volo.Abp.Domain.Repositories;

namespace Grc.Assessments;

/// <summary>
/// Repository interface for Assessment entity
/// </summary>
public interface IAssessmentRepository : IRepository<Assessment, Guid>
{
    Task<Assessment> GetWithControlsAsync(Guid id);
    Task<Assessment> GetWithFrameworksAsync(Guid id);
    Task<List<Assessment>> GetListByStatusAsync(AssessmentStatus status);
    Task<List<Assessment>> GetListByTypeAsync(AssessmentType type);
    Task<List<Assessment>> GetActiveAssessmentsAsync();
    Task<Assessment> GetWithProgressAsync(Guid id);
}

