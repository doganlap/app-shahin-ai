using System;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Assessments;

/// <summary>
/// Control DTO for control assessment context
/// </summary>
public class ControlDto
{
    public Guid Id { get; set; }
    public string ControlNumber { get; set; }
    public LocalizedString Title { get; set; }
    public LocalizedString Requirement { get; set; }
    public ControlType Type { get; set; }
    public Priority Priority { get; set; }
}

