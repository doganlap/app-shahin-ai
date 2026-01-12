using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.FrameworkLibrary.Application.Contracts.Frameworks;
using Grc.FrameworkLibrary.Application.Contracts.Regulators;
using Grc.FrameworkLibrary.Domain.Frameworks;
using Grc.FrameworkLibrary.Domain.Regulators;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.FrameworkLibrary.Application.Frameworks;

/// <summary>
/// Application service for Framework operations
/// </summary>
public class FrameworkAppService : ApplicationService, IFrameworkAppService
{
    private readonly IRepository<Framework, Guid> _frameworkRepository;
    private readonly IRepository<Control, Guid> _controlRepository;
    private readonly IRepository<Regulator, Guid> _regulatorRepository;

    public FrameworkAppService(
        IRepository<Framework, Guid> frameworkRepository,
        IRepository<Control, Guid> controlRepository,
        IRepository<Regulator, Guid> regulatorRepository)
    {
        _frameworkRepository = frameworkRepository;
        _controlRepository = controlRepository;
        _regulatorRepository = regulatorRepository;
    }

    public async Task<PagedResultDto<FrameworkDto>> GetListAsync(GetFrameworkListInput input)
    {
        var queryable = await _frameworkRepository.GetQueryableAsync();

        // Apply filters
        if (input.RegulatorId.HasValue)
        {
            queryable = queryable.Where(f => f.RegulatorId == input.RegulatorId.Value);
        }

        if (input.Category.HasValue)
        {
            queryable = queryable.Where(f => f.Category == input.Category.Value);
        }

        if (input.Status.HasValue)
        {
            queryable = queryable.Where(f => f.Status == input.Status.Value);
        }

        if (input.IsMandatory.HasValue)
        {
            queryable = queryable.Where(f => f.IsMandatory == input.IsMandatory.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(f => 
                f.Code.Contains(input.Filter) || 
                f.Title.En.Contains(input.Filter) || 
                f.Title.Ar.Contains(input.Filter));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(input.Sorting))
        {
            // Simple sorting implementation
            queryable = queryable.OrderBy(f => f.Code);
        }
        else
        {
            queryable = queryable.OrderBy(f => f.Code);
        }
        
        // Apply paging
        queryable = queryable
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var frameworks = await AsyncExecuter.ToListAsync(queryable);
        var frameworkDtos = ObjectMapper.Map<List<Framework>, List<FrameworkDto>>(frameworks);

        // Load regulators for each framework
        var regulatorIds = frameworks.Select(f => f.RegulatorId).Distinct().ToList();
        var regulators = await _regulatorRepository.GetListAsync(r => regulatorIds.Contains(r.Id));
        var regulatorDict = regulators.ToDictionary(r => r.Id);

        foreach (var dto in frameworkDtos)
        {
            if (regulatorDict.TryGetValue(dto.RegulatorId, out var regulator))
            {
                dto.Regulator = ObjectMapper.Map<Regulator, RegulatorDto>(regulator);
            }
        }

        return new PagedResultDto<FrameworkDto>(totalCount, frameworkDtos);
    }

    public async Task<FrameworkDto> GetAsync(Guid id)
    {
        var framework = await _frameworkRepository.GetAsync(id, includeDetails: true);
        var dto = ObjectMapper.Map<Framework, FrameworkDto>(framework);

        // Load regulator
        if (framework.RegulatorId != Guid.Empty)
        {
            var regulator = await _regulatorRepository.GetAsync(framework.RegulatorId);
            dto.Regulator = ObjectMapper.Map<Regulator, RegulatorDto>(regulator);
        }

        return dto;
    }

    public async Task<PagedResultDto<ControlDto>> GetControlsAsync(Guid frameworkId, GetControlListInput input)
    {
        var queryable = await _controlRepository.GetQueryableAsync();
        queryable = queryable.Where(c => c.FrameworkId == frameworkId);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(input.DomainCode))
        {
            queryable = queryable.Where(c => c.DomainCode == input.DomainCode);
        }

        if (input.ControlType.HasValue)
        {
            queryable = queryable.Where(c => c.Type == input.ControlType.Value);
        }

        if (input.Category.HasValue)
        {
            queryable = queryable.Where(c => c.Category == input.Category.Value);
        }

        if (input.Priority.HasValue)
        {
            queryable = queryable.Where(c => c.Priority == input.Priority.Value);
        }

        if (input.MinMaturityLevel.HasValue)
        {
            queryable = queryable.Where(c => c.MaturityLevel >= input.MinMaturityLevel.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(c => 
                c.ControlNumber.Contains(input.Filter) || 
                c.Title.En.Contains(input.Filter) || 
                c.Title.Ar.Contains(input.Filter));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(input.Sorting))
        {
            queryable = queryable.OrderBy(c => c.ControlNumber);
        }
        else
        {
            queryable = queryable.OrderBy(c => c.ControlNumber);
        }
        
        // Apply paging
        queryable = queryable
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var controls = await AsyncExecuter.ToListAsync(queryable);
        var controlDtos = ObjectMapper.Map<List<Control>, List<ControlDto>>(controls);

        return new PagedResultDto<ControlDto>(totalCount, controlDtos);
    }

    public async Task<List<ControlDto>> SearchControlsAsync(string query, List<Guid> frameworkIds = null)
    {
        var queryable = await _controlRepository.GetQueryableAsync();

        if (frameworkIds != null && frameworkIds.Any())
        {
            queryable = queryable.Where(c => frameworkIds.Contains(c.FrameworkId));
        }

        // Full-text search on title and requirement
        queryable = queryable.Where(c => 
            c.Title.En.Contains(query) || 
            c.Title.Ar.Contains(query) || 
            c.Requirement.En.Contains(query) || 
            c.Requirement.Ar.Contains(query) ||
            c.ControlNumber.Contains(query));

        var controls = await AsyncExecuter.ToListAsync(queryable);
        return ObjectMapper.Map<List<Control>, List<ControlDto>>(controls);
    }
}

