using System;
using Grc.Enums;

namespace Grc.Product.Application.Contracts.Subscriptions;

/// <summary>
/// Quota usage DTO
/// </summary>
public class QuotaUsageDto
{
    public Guid Id { get; set; }
    public QuotaType QuotaType { get; set; }
    public decimal CurrentUsage { get; set; }
    public decimal? Limit { get; set; }
    public DateTime? ResetDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public decimal PercentageUsed { get; set; }
    public bool IsExceeded { get; set; }
}
