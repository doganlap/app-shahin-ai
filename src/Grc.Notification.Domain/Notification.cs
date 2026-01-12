using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Notification;

/// <summary>
/// Notification entity
/// </summary>
public class Notification : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string RecipientUserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string NotificationType { get; set; }
    public string Channel { get; set; } // Email, SMS, InApp, Push
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime SentAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    
    protected Notification() { }
    
    public Notification(Guid id, string recipientUserId, string title, string message, string notificationType, string channel)
        : base(id)
    {
        RecipientUserId = recipientUserId;
        Title = title;
        Message = message;
        NotificationType = notificationType;
        Channel = channel;
        IsRead = false;
        SentAt = DateTime.UtcNow;
        Metadata = new Dictionary<string, object>();
    }
    
    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}

