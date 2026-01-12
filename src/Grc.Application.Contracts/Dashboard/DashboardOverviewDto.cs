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
    public string ComplianceLevel { get; set; }
    public List<UpcomingDeadlineDto> UpcomingDeadlines { get; set; }
}

/// <summary>
/// Upcoming deadline information
/// </summary>
public class UpcomingDeadlineDto
{
    public string Name { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysRemaining { get; set; }
}

