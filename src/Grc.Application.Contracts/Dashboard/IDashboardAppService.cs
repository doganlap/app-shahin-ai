using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Grc.Dashboard;

/// <summary>
/// Dashboard application service interface
/// </summary>
public interface IDashboardAppService : IApplicationService
{
    Task<DashboardOverviewDto> GetOverviewAsync();
    Task<List<MyControlDto>> GetMyControlsAsync();
    Task<List<ControlAssessmentDto>> GetPendingVerificationAsync();
    Task<List<FrameworkProgressDto>> GetFrameworkProgressAsync(Guid? assessmentId = null);
}

