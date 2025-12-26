using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Permissions;
using Grc.Risk.Application.Contracts.Risks;
using Grc.Risk.Domain.Risks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.Risk.Application.Risks;

/// <summary>
/// Application service for Risk operations
/// </summary>
[Authorize(GrcPermissions.Risks.Default)]
public class RiskAppService : ApplicationService, IRiskAppService
{
    private readonly IRepository<Risk, Guid> _riskRepository;
    private readonly IRepository<RiskTreatment, Guid> _treatmentRepository;

    public RiskAppService(
        IRepository<Risk, Guid> riskRepository,
        IRepository<RiskTreatment, Guid> treatmentRepository)
    {
        _riskRepository = riskRepository;
        _treatmentRepository = treatmentRepository;
    }

    [Authorize(GrcPermissions.Risks.Create)]
    public async Task<RiskDto> CreateAsync(CreateRiskInput input)
    {
        var risk = new Risk(
            GuidGenerator.Create(),
            input.RiskCode,
            input.Title,
            input.Category)
        {
            TenantId = CurrentTenant.Id
        };

        if (input.Description != null)
        {
            risk.SetDescription(input.Description);
        }

        await _riskRepository.InsertAsync(risk);
        return ObjectMapper.Map<Risk, RiskDto>(risk);
    }

    [Authorize(GrcPermissions.Risks.View)]
    public async Task<RiskDto> GetAsync(Guid id)
    {
        var risk = await _riskRepository.GetAsync(id);
        var dto = ObjectMapper.Map<Risk, RiskDto>(risk);
        
        // Load treatments
        var treatments = await _treatmentRepository.GetListAsync(t => t.RiskId == id);
        dto.Treatments = ObjectMapper.Map<List<RiskTreatment>, List<RiskTreatmentDto>>(treatments);
        
        return dto;
    }

    [Authorize(GrcPermissions.Risks.Assess)]
    public async Task<RiskDto> AssessAsync(Guid id, AssessRiskInput input)
    {
        var risk = await _riskRepository.GetAsync(id);
        risk.Assess(input.Probability, input.Impact);
        
        await _riskRepository.UpdateAsync(risk);
        return ObjectMapper.Map<Risk, RiskDto>(risk);
    }

    [Authorize(GrcPermissions.Risks.Treat)]
    public async Task<RiskDto> ApplyTreatmentAsync(Guid id, ApplyTreatmentInput input)
    {
        var risk = await _riskRepository.GetAsync(id);
        risk.ApplyTreatment(input.ResidualProbability, input.ResidualImpact);
        
        if (!string.IsNullOrWhiteSpace(input.TreatmentDescription))
        {
            var treatment = new RiskTreatment(
                GuidGenerator.Create(),
                id,
                "Mitigate",
                input.TreatmentDescription);
            
            await _treatmentRepository.InsertAsync(treatment);
        }
        
        await _riskRepository.UpdateAsync(risk);
        return ObjectMapper.Map<Risk, RiskDto>(risk);
    }

    [Authorize(GrcPermissions.Risks.View)]
    public async Task<List<RiskDto>> GetListAsync(GetRiskListInput input)
    {
        var queryable = await _riskRepository.GetQueryableAsync();
        
        // Apply filters
        if (input.Category.HasValue)
        {
            queryable = queryable.Where(r => r.Category == input.Category.Value);
        }
        
        if (input.Status.HasValue)
        {
            queryable = queryable.Where(r => r.Status == input.Status.Value);
        }
        
        if (input.RiskLevel.HasValue)
        {
            queryable = queryable.Where(r => r.InherentRiskLevel == input.RiskLevel.Value);
        }
        
        if (input.RiskOwnerId.HasValue)
        {
            queryable = queryable.Where(r => r.RiskOwnerId == input.RiskOwnerId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(r => 
                r.RiskCode.Contains(input.Filter) ||
                r.Title.En.Contains(input.Filter) ||
                r.Title.Ar.Contains(input.Filter));
        }
        
        var risks = await AsyncExecuter.ToListAsync(queryable);
        return ObjectMapper.Map<List<Risk>, List<RiskDto>>(risks);
    }
}

