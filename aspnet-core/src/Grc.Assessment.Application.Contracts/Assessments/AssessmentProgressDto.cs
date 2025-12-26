using System;
using System.Collections.Generic;
using Grc.Enums;

namespace Grc.Assessments;

/// <summary>
/// Assessment progress metrics
/// </summary>
public class AssessmentProgressDto
{
    public int TotalControls { get; set; }
    public int CompletedControls { get; set; }
    public int InProgressControls { get; set; }
    public int NotStartedControls { get; set; }
    public int OverdueControls { get; set; }
    public decimal CompletionPercentage { get; set; }
    public decimal OverallScore { get; set; }
    public List<FrameworkProgressDto> ByFramework { get; set; }
    public Dictionary<string, int> ByStatus { get; set; }
}

