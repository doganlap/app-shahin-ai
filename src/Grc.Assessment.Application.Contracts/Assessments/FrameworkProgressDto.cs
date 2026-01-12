using System;

namespace Grc.Assessments;

/// <summary>
/// Framework progress within an assessment
/// </summary>
public class FrameworkProgressDto
{
    public Guid FrameworkId { get; set; }
    public string FrameworkCode { get; set; }
    public string FrameworkName { get; set; }
    public int TotalControls { get; set; }
    public int CompletedControls { get; set; }
    public decimal CompletionPercentage { get; set; }
    public decimal AverageScore { get; set; }
}

