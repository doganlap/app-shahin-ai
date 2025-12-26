using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Assessments;
using Grc.Dashboard;
using Grc.Enums;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.Application.Dashboard;

/// <summary>
/// Dashboard application service
/// </summary>
[Authorize(GrcPermissions.Reports.View)]
public class DashboardAppService : ApplicationService, IDashboardAppService
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IControlAssessmentRepository _controlAssessmentRepository;

    public DashboardAppService(
        IAssessmentRepository assessmentRepository,
        IControlAssessmentRepository controlAssessmentRepository)
    {
        _assessmentRepository = assessmentRepository;
        _controlAssessmentRepository = controlAssessmentRepository;
    }

    public async Task<DashboardOverviewDto> GetOverviewAsync()
    {
        var activeAssessments = await _assessmentRepository.GetListByStatusAsync(AssessmentStatus.InProgress);
        var allControlAssessments = new List<ControlAssessment>();
        
        foreach (var assessment in activeAssessments)
        {
            var controls = await _controlAssessmentRepository.GetByAssessmentAsync(assessment.Id);
            allControlAssessments.AddRange(controls);
        }

        var overview = new DashboardOverviewDto
        {
            ActiveAssessments = activeAssessments.Count,
            TotalControls = allControlAssessments.Count,
            CompletedControls = allControlAssessments.Count(c => c.IsComplete),
            OverdueControls = allControlAssessments.Count(c => c.IsOverdue),
            AverageScore = allControlAssessments
                .Where(c => c.VerifiedScore.HasValue)
                .Select(c => c.VerifiedScore!.Value)
                .DefaultIfEmpty(0)
                .Average(),
            ComplianceLevel = CalculateComplianceLevel(allControlAssessments),
            UpcomingDeadlines = await GetUpcomingDeadlinesAsync()
        };

        return overview;
    }

    [Authorize(GrcPermissions.ControlAssessments.ViewOwn)]
    public async Task<List<MyControlDto>> GetMyControlsAsync()
    {
        var currentUserId = CurrentUser.Id;
        if (!currentUserId.HasValue)
        {
            return new List<MyControlDto>();
        }

        var myControls = await _controlAssessmentRepository.GetByAssignedUserAsync(currentUserId.Value);
        
        // TODO: Load control details and framework information
        return myControls.Select(c => new MyControlDto
        {
            Id = c.Id,
            ControlNumber = "N/A", // TODO: Load from control
            ControlTitle = new ValueObjects.LocalizedString { En = "Control", Ar = "ضابط" },
            FrameworkCode = "N/A", // TODO: Load from framework
            Status = c.Status,
            DueDate = c.DueDate,
            IsOverdue = c.IsOverdue,
            Priority = c.Priority
        }).ToList();
    }

    [Authorize(GrcPermissions.ControlAssessments.Verify)]
    public async Task<List<ControlAssessmentDto>> GetPendingVerificationAsync()
    {
        var pending = await _controlAssessmentRepository.GetPendingVerificationAsync();
        
        // TODO: Load control and assessment details
        return pending.Select(c => new ControlAssessmentDto
        {
            Id = c.Id,
            AssessmentId = c.AssessmentId,
            ControlId = c.ControlId,
            Status = c.Status,
            SelfScore = c.SelfScore,
            DueDate = c.DueDate,
            Priority = c.Priority
        }).ToList();
    }

    public async Task<List<FrameworkProgressDto>> GetFrameworkProgressAsync(Guid? assessmentId = null)
    {
        // TODO: Implement framework progress calculation
        // This would require joining assessments, frameworks, and control assessments
        return new List<FrameworkProgressDto>();
    }

    private string CalculateComplianceLevel(List<ControlAssessment> controls)
    {
        if (!controls.Any()) return "Not Started";
        
        var completed = controls.Count(c => c.IsComplete);
        var percentage = (decimal)completed / controls.Count * 100;
        
        return percentage switch
        {
            >= 90 => "Excellent",
            >= 75 => "Good",
            >= 50 => "Fair",
            >= 25 => "Poor",
            _ => "Critical"
        };
    }

    private async Task<List<UpcomingDeadlineDto>> GetUpcomingDeadlinesAsync()
    {
        var overdue = await _controlAssessmentRepository.GetOverdueAsync();
        var upcoming = overdue
            .Where(c => c.DueDate.HasValue && c.DueDate.Value <= DateTime.UtcNow.AddDays(7))
            .OrderBy(c => c.DueDate)
            .Take(10)
            .Select(c => new UpcomingDeadlineDto
            {
                Name = $"Control Assessment {c.Id}", // TODO: Load control name
                DueDate = c.DueDate!.Value,
                DaysRemaining = (int)(c.DueDate.Value - DateTime.UtcNow).TotalDays
            })
            .ToList();

        return upcoming;
    }
}

