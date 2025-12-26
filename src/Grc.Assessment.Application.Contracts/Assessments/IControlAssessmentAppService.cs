using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Grc.Assessments;

/// <summary>
/// Application service interface for ControlAssessment operations
/// </summary>
public interface IControlAssessmentAppService : IApplicationService
{
    Task<ControlAssessmentDetailDto> GetAsync(Guid id);
    Task<ControlAssessmentDto> AssignAsync(Guid id, AssignControlInput input);
    Task<BulkOperationResult> BulkAssignAsync(BulkAssignControlsInput input);
    Task<ControlAssessmentDto> SubmitScoreAsync(Guid id, SubmitScoreInput input);
    Task<ControlAssessmentDto> VerifyAsync(Guid id, VerifyControlInput input);
    Task<ControlAssessmentDto> RejectAsync(Guid id, RejectControlInput input);
}

