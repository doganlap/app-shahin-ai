using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Grc.Web.Pages.Assessments;

[Authorize(GrcPermissions.Assessments.Default)]
public class IndexModel : GrcPageModel
{
    public List<AssessmentListItem> Assessments { get; set; } = new();
    public int TotalCount { get; set; }
    public AssessmentSummary Summary { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Sample data for demonstration - replace with actual service calls when available
        Summary = new AssessmentSummary
        {
            TotalAssessments = 12,
            InProgress = 5,
            Completed = 4,
            Draft = 3
        };

        Assessments = new List<AssessmentListItem>
        {
            new() { Id = Guid.NewGuid(), Name = "Annual ISO 27001 Assessment", Description = "Comprehensive ISO 27001 compliance assessment", FrameworkName = "ISO 27001:2022", Type = "Compliance", Status = "InProgress", Progress = 65, CompletedControls = 78, TotalControls = 120, Score = 78, OwnerName = "John Smith", StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(2) },
            new() { Id = Guid.NewGuid(), Name = "Q4 SAMA Compliance Review", Description = "Quarterly SAMA CSF compliance review", FrameworkName = "SAMA CSF", Type = "Regulatory", Status = "InProgress", Progress = 45, CompletedControls = 45, TotalControls = 100, Score = 82, OwnerName = "Jane Doe", StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(1) },
            new() { Id = Guid.NewGuid(), Name = "GDPR Data Protection Assessment", Description = "GDPR compliance assessment for EU operations", FrameworkName = "GDPR", Type = "Privacy", Status = "Completed", Progress = 100, CompletedControls = 50, TotalControls = 50, Score = 91, OwnerName = "Ahmed Ali", StartDate = DateTime.Now.AddMonths(-4), EndDate = DateTime.Now.AddMonths(-1) },
            new() { Id = Guid.NewGuid(), Name = "NCA ECC Assessment", Description = "NCA Essential Cybersecurity Controls assessment", FrameworkName = "NCA ECC", Type = "Regulatory", Status = "Draft", Progress = 0, CompletedControls = 0, TotalControls = 85, Score = 0, OwnerName = null, StartDate = null, EndDate = DateTime.Now.AddMonths(3) },
            new() { Id = Guid.NewGuid(), Name = "SOC 2 Type II Readiness", Description = "SOC 2 Type II audit readiness assessment", FrameworkName = "SOC 2", Type = "Audit", Status = "InProgress", Progress = 30, CompletedControls = 25, TotalControls = 83, Score = 68, OwnerName = "Sara Khan", StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddMonths(4) }
        };

        TotalCount = Assessments.Count;
        await Task.CompletedTask;
    }
}

public class AssessmentListItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FrameworkName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int CompletedControls { get; set; }
    public int TotalControls { get; set; }
    public decimal Score { get; set; }
    public string? OwnerName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class AssessmentSummary
{
    public int TotalAssessments { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Draft { get; set; }
}
