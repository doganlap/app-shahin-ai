using System;

namespace Grc.Product.Subscriptions;

public class QuotaUsageDto
{
    public string QuotaType { get; set; }
    public decimal CurrentUsage { get; set; }
    public decimal? Limit { get; set; }
    public string Unit { get; set; }
    public decimal? Remaining { get; set; }
    public DateTime? ResetDate { get; set; }
    public decimal? PercentageUsed { get; set; }
    public bool IsUnlimited { get; set; }
}


