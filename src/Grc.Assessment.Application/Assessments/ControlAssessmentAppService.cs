using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Assessments;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.Assessment.Application.Assessments;

/// <summary>
/// Application service for ControlAssessment operations
/// </summary>
[Authorize(GrcPermissions.ControlAssessments.Default)]
public class ControlAssessmentAppService : ApplicationService, IControlAssessmentAppService
{
    private readonly IControlAssessmentRepository _controlAssessmentRepository;
    private readonly IAssessmentRepository _assessmentRepository;

    public ControlAssessmentAppService(
        IControlAssessmentRepository controlAssessmentRepository,
        IAssessmentRepository assessmentRepository)
    {
        _controlAssessmentRepository = controlAssessmentRepository;
        _assessmentRepository = assessmentRepository;
    }

    [Authorize(GrcPermissions.ControlAssessments.ViewOwn)]
    public async Task<ControlAssessmentDetailDto> GetAsync(Guid id)
    {
        var controlAssessment = await _controlAssessmentRepository.GetWithEvidenceAsync(id);
        
        // Check permissions - user can view own, department, or all based on permission
        // TODO: Implement permission checks based on assignment
        
        var dto = ObjectMapper.Map<ControlAssessment, ControlAssessmentDetailDto>(controlAssessment);
        
        // Load comments and history
        dto.Comments = ObjectMapper.Map<List<ControlAssessmentComment>, List<CommentDto>>(
            controlAssessment.Comments.ToList());
        dto.History = ObjectMapper.Map<List<ControlAssessmentHistory>, List<HistoryDto>>(
            controlAssessment.History.ToList());
        dto.Evidences = ObjectMapper.Map<List<Evidence.Evidence>, List<EvidenceDto>>(
            controlAssessment.Evidences.ToList());
        
        return dto;
    }

    [Authorize(GrcPermissions.Assessments.AssignControls)]
    public async Task<ControlAssessmentDto> AssignAsync(Guid id, AssignControlInput input)
    {
        var controlAssessment = await _controlAssessmentRepository.GetAsync(id);
        controlAssessment.AssignTo(input.UserId, input.DueDate);
        
        if (input.Priority.HasValue)
        {
            controlAssessment.SetPriority(input.Priority.Value);
        }
        
        await _controlAssessmentRepository.UpdateAsync(controlAssessment);
        return ObjectMapper.Map<ControlAssessment, ControlAssessmentDto>(controlAssessment);
    }

    [Authorize(GrcPermissions.Assessments.AssignControls)]
    public async Task<BulkOperationResult> BulkAssignAsync(BulkAssignControlsInput input)
    {
        var result = new BulkOperationResult
        {
            Errors = new List<BulkOperationError>()
        };

        foreach (var assignment in input.Assignments)
        {
            try
            {
                var controlAssessment = await _controlAssessmentRepository.GetAsync(assignment.ControlAssessmentId);
                controlAssessment.AssignTo(assignment.UserId, assignment.DueDate);
                await _controlAssessmentRepository.UpdateAsync(controlAssessment);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailureCount++;
                result.Errors.Add(new BulkOperationError
                {
                    Id = assignment.ControlAssessmentId,
                    Error = ex.Message
                });
            }
        }

        return result;
    }

    [Authorize(GrcPermissions.ControlAssessments.UpdateOwn)]
    public async Task<ControlAssessmentDto> SubmitScoreAsync(Guid id, SubmitScoreInput input)
    {
        var controlAssessment = await _controlAssessmentRepository.GetAsync(id);
        
        // TODO: Verify user owns this control assessment
        
        controlAssessment.SubmitSelfScore(input.Score, input.Notes);
        await _controlAssessmentRepository.UpdateAsync(controlAssessment);
        
        return ObjectMapper.Map<ControlAssessment, ControlAssessmentDto>(controlAssessment);
    }

    [Authorize(GrcPermissions.ControlAssessments.Verify)]
    public async Task<ControlAssessmentDto> VerifyAsync(Guid id, VerifyControlInput input)
    {
        var controlAssessment = await _controlAssessmentRepository.GetAsync(id);
        var currentUser = CurrentUser.Id ?? Guid.Empty;
        
        controlAssessment.Verify(currentUser, input.Score);
        await _controlAssessmentRepository.UpdateAsync(controlAssessment);
        
        // Update parent assessment progress
        var assessment = await _assessmentRepository.GetAsync(controlAssessment.AssessmentId);
        // Assessment will recalculate overall score on next read
        
        return ObjectMapper.Map<ControlAssessment, ControlAssessmentDto>(controlAssessment);
    }

    [Authorize(GrcPermissions.ControlAssessments.Reject)]
    public async Task<ControlAssessmentDto> RejectAsync(Guid id, RejectControlInput input)
    {
        var controlAssessment = await _controlAssessmentRepository.GetAsync(id);
        var currentUser = CurrentUser.Id ?? Guid.Empty;
        
        controlAssessment.Reject(currentUser, input.Reason);
        await _controlAssessmentRepository.UpdateAsync(controlAssessment);
        
        return ObjectMapper.Map<ControlAssessment, ControlAssessmentDto>(controlAssessment);
    }
}

