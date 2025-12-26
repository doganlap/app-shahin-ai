using System;
using Grc.Enums;

namespace Grc.Risk.Application.Contracts.Risks;

/// <summary>
/// Risk treatment DTO
/// </summary>
public class RiskTreatmentDto
{
    public Guid Id { get; set; }
    public string TreatmentType { get; set; }
    public string Description { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? TargetDate { get; set; }
    public string Status { get; set; }
}

