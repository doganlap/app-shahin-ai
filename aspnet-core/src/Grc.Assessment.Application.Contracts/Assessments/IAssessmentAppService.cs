using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Grc.Assessments;

/// <summary>
/// Application service interface for Assessment operations
/// </summary>
public interface IAssessmentAppService : IApplicationService
{
    Task<AssessmentDto> CreateAsync(CreateAssessmentInput input);
    Task<AssessmentDto> GenerateAsync(GenerateAssessmentInput input);
    Task<AssessmentDto> GetAsync(Guid id);
    Task<AssessmentProgressDto> GetProgressAsync(Guid id);
    Task<AssessmentDto> StartAsync(Guid id);
    Task<AssessmentDto> CompleteAsync(Guid id);
}

