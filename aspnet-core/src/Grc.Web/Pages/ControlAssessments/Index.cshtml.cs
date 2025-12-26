using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Enums;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Grc.Web.Pages.ControlAssessments;

[Authorize(GrcPermissions.ControlAssessments.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public List<ControlAssessmentListItem> ControlAssessments { get; set; } = new();
    public int TotalCount { get; set; }
    public ControlAssessmentSummary Summary { get; set; } = new();

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        // Query control assessments from database with control info
        var assessments = await _dbContext.ControlAssessments
            .Where(ca => !ca.IsDeleted)
            .OrderByDescending(ca => ca.CreationTime)
            .ToListAsync();

        // Get control info for display
        var controlIds = assessments.Select(a => a.ControlId).Distinct().ToList();
        var controls = await _dbContext.Controls
            .Where(c => controlIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => new { c.ControlNumber, Title = c.Title?.En ?? c.Title?.Ar ?? "" });

        ControlAssessments = assessments.Select(ca => {
            var control = controls.GetValueOrDefault(ca.ControlId);
            return new ControlAssessmentListItem
            {
                Id = ca.Id,
                ControlCode = control?.ControlNumber ?? "Unknown",
                ControlTitle = control?.Title ?? "Unknown Control",
                AssessmentName = $"Assessment {ca.AssessmentId.ToString().Substring(0, 8)}",
                Status = ca.Status.ToString(),
                ComplianceScore = (int)(ca.VerifiedScore ?? ca.SelfScore ?? 0),
                LastAssessedDate = ca.VerificationDate ?? ca.CreationTime,
                AssessorName = "Assigned Team"
            };
        }).ToList();

        TotalCount = ControlAssessments.Count;

        // Calculate summary
        Summary = new ControlAssessmentSummary
        {
            Total = assessments.Count,
            NotStarted = assessments.Count(a => a.Status == ControlAssessmentStatus.NotStarted),
            InProgress = assessments.Count(a => a.Status == ControlAssessmentStatus.InProgress || a.Status == ControlAssessmentStatus.PendingReview),
            Complete = assessments.Count(a => a.Status == ControlAssessmentStatus.Verified)
        };
    }
}

public class ControlAssessmentListItem
{
    public Guid Id { get; set; }
    public string ControlCode { get; set; } = string.Empty;
    public string ControlTitle { get; set; } = string.Empty;
    public string AssessmentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ComplianceScore { get; set; }
    public DateTime? LastAssessedDate { get; set; }
    public string AssessorName { get; set; } = string.Empty;
}

public class ControlAssessmentSummary
{
    public int Total { get; set; }
    public int NotStarted { get; set; }
    public int InProgress { get; set; }
    public int Complete { get; set; }
}
