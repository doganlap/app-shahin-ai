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
    /// <summary>
    /// Get dashboard overview metrics
    /// </summary>
    Task<DashboardOverviewDto> GetOverviewAsync();

    /// <summary>
    /// Get user's assigned controls
    /// </summary>
    Task<List<MyControlDto>> GetMyControlsAsync();

    /// <summary>
    /// Get framework compliance progress
    /// </summary>
    /// <param name="assessmentId">Optional assessment ID filter</param>
    Task<List<FrameworkProgressDto>> GetFrameworkProgressAsync(Guid? assessmentId = null);

    /// <summary>
    /// Get items pending verification
    /// </summary>
    Task<List<object>> GetPendingVerificationAsync();
}
