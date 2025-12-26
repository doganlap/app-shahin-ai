using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Enums;
using Volo.Abp.Domain.Repositories;

namespace Grc.Assessments;

/// <summary>
/// Repository interface for ControlAssessment entity
/// </summary>
public interface IControlAssessmentRepository : IRepository<ControlAssessment, Guid>
{
    Task<ControlAssessment> GetWithEvidenceAsync(Guid id);
    Task<ControlAssessment> GetWithCommentsAsync(Guid id);
    Task<List<ControlAssessment>> GetByAssessmentAsync(Guid assessmentId);
    Task<List<ControlAssessment>> GetByAssignedUserAsync(Guid userId);
    Task<List<ControlAssessment>> GetByStatusAsync(Guid assessmentId, ControlAssessmentStatus status);
    Task<List<ControlAssessment>> GetOverdueAsync(Guid? assessmentId = null);
    Task<List<ControlAssessment>> GetPendingVerificationAsync();
}

