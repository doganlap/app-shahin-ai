using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Product.Subscriptions;

/// <summary>
/// Track quota usage for a tenant
/// </summary>
public class QuotaUsage : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Enums.QuotaType QuotaType { get; private set; }
    public decimal CurrentUsage { get; private set; }
    public DateTime? ResetDate { get; private set; }
    public DateTime LastUpdated { get; private set; }
    
    protected QuotaUsage() { }
    
    public QuotaUsage(Guid id, Guid tenantId, Enums.QuotaType quotaType)
        : base(id)
    {
        TenantId = tenantId;
        QuotaType = quotaType;
        CurrentUsage = 0;
        LastUpdated = DateTime.UtcNow;
    }
    
    public void Increment(decimal amount)
    {
        CurrentUsage += amount;
        LastUpdated = DateTime.UtcNow;
    }
    
    public void Decrement(decimal amount)
    {
        CurrentUsage = Math.Max(0, CurrentUsage - amount);
        LastUpdated = DateTime.UtcNow;
    }
    
    public void Reset()
    {
        CurrentUsage = 0;
        LastUpdated = DateTime.UtcNow;
    }
    
    public void SetResetDate(DateTime? resetDate)
    {
        ResetDate = resetDate;
    }
    
    public void SetUsage(decimal usage)
    {
        CurrentUsage = usage;
        LastUpdated = DateTime.UtcNow;
    }
}


