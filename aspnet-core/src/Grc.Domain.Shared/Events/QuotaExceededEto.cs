using System;
using Grc.Enums;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when quota is exceeded
/// </summary>
[EventName("Grc.Quota.Exceeded")]
public class QuotaExceededEto
{
    public Guid? TenantId { get; set; }
    public QuotaType QuotaType { get; set; }
    public decimal CurrentUsage { get; set; }
    public decimal Limit { get; set; }
    public DateTime ExceededTime { get; set; }
}

