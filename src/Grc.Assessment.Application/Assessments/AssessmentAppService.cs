using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Assessments;
using Grc.Enums;
using Grc.Permissions;
using Grc.Product.Services;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.Assessment.Application.Assessments;

/// <summary>
/// Application service for Assessment operations
/// </summary>
[Authorize(GrcPermissions.Assessments.Default)]
public class AssessmentAppService : ApplicationService, IAssessmentAppService
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IControlAssessmentRepository _controlAssessmentRepository;
    private readonly QuotaEnforcementService _quotaEnforcementService;

    public AssessmentAppService(
        IAssessmentRepository assessmentRepository,
        IControlAssessmentRepository controlAssessmentRepository,
        QuotaEnforcementService quotaEnforcementService)
    {
        _assessmentRepository = assessmentRepository;
        _controlAssessmentRepository = controlAssessmentRepository;
        _quotaEnforcementService = quotaEnforcementService;
    }

    [Authorize(GrcPermissions.Assessments.Create)]
    public async Task<AssessmentDto> CreateAsync(CreateAssessmentInput input)
    {
        var tenantId = CurrentTenant.Id;
        if (tenantId.HasValue)
        {
            // Check quota for assessments
            var allowed = await _quotaEnforcementService.CheckQuotaAsync(
                tenantId.Value, 
                QuotaType.Assessments, 
                1);
            
            if (!allowed)
            {
                throw new InvalidOperationException("Assessment quota limit exceeded. Please upgrade your subscription.");
            }
        }

        var assessment = new Assessment(
            GuidGenerator.Create(),
            input.Name,
            input.Type,
            input.StartDate,
            input.TargetEndDate);

        if (!string.IsNullOrWhiteSpace(input.Description))
        {
            assessment.SetDescription(input.Description);
        }

        // Add frameworks
        foreach (var frameworkId in input.FrameworkIds)
        {
            // TODO: Load framework and add controls
            // var framework = await _frameworkRepository.GetAsync(frameworkId);
            // assessment.AddFramework(frameworkId);
        }

        await _assessmentRepository.InsertAsync(assessment);

        // Reserve quota
        if (tenantId.HasValue)
        {
            await _quotaEnforcementService.ReserveQuotaAsync(tenantId.Value, QuotaType.Assessments, 1);
        }

        return ObjectMapper.Map<Assessment, AssessmentDto>(assessment);
    }

    [Authorize(GrcPermissions.Assessments.Generate)]
    public async Task<AssessmentDto> GenerateAsync(GenerateAssessmentInput input)
    {
        // TODO: Implement assessment template generator
        // This will use the AssessmentTemplateGenerator service
        // to automatically determine applicable frameworks based on tenant profile
        
        throw new NotImplementedException("AssessmentTemplateGenerator not yet implemented");
    }

    [Authorize(GrcPermissions.Assessments.View)]
    public async Task<AssessmentDto> GetAsync(Guid id)
    {
        var assessment = await _assessmentRepository.GetWithControlsAsync(id);
        return ObjectMapper.Map<Assessment, AssessmentDto>(assessment);
    }

    [Authorize(GrcPermissions.Assessments.View)]
    public async Task<AssessmentProgressDto> GetProgressAsync(Guid id)
    {
        var assessment = await _assessmentRepository.GetWithProgressAsync(id);
        var controlAssessments = await _controlAssessmentRepository.GetByAssessmentAsync(id);

        var progress = new AssessmentProgressDto
        {
            TotalControls = assessment.TotalControls,
            CompletedControls = assessment.CompletedControls,
            InProgressControls = controlAssessments.Count(c => c.Status == ControlAssessmentStatus.InProgress),
            NotStartedControls = controlAssessments.Count(c => c.Status == ControlAssessmentStatus.NotStarted),
            OverdueControls = controlAssessments.Count(c => c.IsOverdue),
            CompletionPercentage = assessment.CompletionPercentage,
            OverallScore = assessment.OverallScore,
            ByFramework = new List<FrameworkProgressDto>(),
            ByStatus = controlAssessments
                .GroupBy(c => c.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        // TODO: Calculate by framework progress
        // This would require joining with frameworks and grouping

        return progress;
    }

    [Authorize(GrcPermissions.Assessments.Edit)]
    public async Task<AssessmentDto> StartAsync(Guid id)
    {
        var assessment = await _assessmentRepository.GetAsync(id);
        assessment.Start();
        await _assessmentRepository.UpdateAsync(assessment);
        return ObjectMapper.Map<Assessment, AssessmentDto>(assessment);
    }

    [Authorize(GrcPermissions.Assessments.Edit)]
    public async Task<AssessmentDto> CompleteAsync(Guid id)
    {
        var assessment = await _assessmentRepository.GetAsync(id);
        assessment.Complete();
        await _assessmentRepository.UpdateAsync(assessment);
        return ObjectMapper.Map<Assessment, AssessmentDto>(assessment);
    }
}

