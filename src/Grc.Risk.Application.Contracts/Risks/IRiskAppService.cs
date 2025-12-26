using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Grc.Risk.Application.Contracts.Risks;

/// <summary>
/// Application service interface for Risk operations
/// </summary>
public interface IRiskAppService : IApplicationService
{
    Task<RiskDto> CreateAsync(CreateRiskInput input);
    Task<RiskDto> GetAsync(Guid id);
    Task<RiskDto> AssessAsync(Guid id, AssessRiskInput input);
    Task<RiskDto> ApplyTreatmentAsync(Guid id, ApplyTreatmentInput input);
    Task<List<RiskDto>> GetListAsync(GetRiskListInput input);
}

