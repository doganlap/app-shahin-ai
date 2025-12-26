using System;
using System.Collections.Generic;

namespace Grc.Dashboard;

/// <summary>
/// Dashboard overview metrics
/// </summary>
public class DashboardOverviewDto
{
    public int ActiveAssessments { get; set; }
    public int TotalControls { get; set; }
    public int CompletedControls { get; set; }
    public int OverdueControls { get; set; }
    public decimal AverageScore { get; set; }
    public string ComplianceLevel { get; set; } = string.Empty;
    public List<UpcomingDeadlineDto> UpcomingDeadlines { get; set; } = new();
}

/// <summary>
/// Upcoming deadline information
/// </summary>
public class UpcomingDeadlineDto
{
    public string Name { get; set; } = string.Empty;
    public string DueDate { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
}

/// <summary>
/// User's assigned control
/// </summary>
public class MyControlDto
{
    public string Id { get; set; } = string.Empty;
    public string ControlName { get; set; } = string.Empty;
    public string FrameworkName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DueDate { get; set; } = string.Empty;
    public string AssignedDate { get; set; } = string.Empty;
}

/// <summary>
/// Framework compliance progress
/// </summary>
public class FrameworkProgressDto
{
    public string FrameworkName { get; set; } = string.Empty;
    public int TotalControls { get; set; }
    public int CompletedControls { get; set; }
    public int InProgressControls { get; set; }
    public int NotStartedControls { get; set; }
    public int CompliancePercentage { get; set; }
}
