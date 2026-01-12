using System;
using System.Collections.Generic;
using Grc.Enums;

namespace Grc.Product.Application.Contracts.Subscriptions;

/// <summary>
/// Detailed subscription DTO
/// </summary>
public class SubscriptionDetailDto : TenantSubscriptionDto
{
    public List<QuotaUsageDto> QuotaUsages { get; set; }
    public Dictionary<QuotaType, QuotaStatusDto> QuotaStatuses { get; set; }
}

/// <summary>
/// Quota status DTO
/// </summary>
public class QuotaStatusDto
{
    public decimal CurrentUsage { get; set; }
    public decimal? Limit { get; set; }
    public decimal PercentageUsed { get; set; }
    public bool IsExceeded { get; set; }
    public bool IsUnlimited { get; set; }
}
