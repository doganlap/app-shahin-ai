using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Notification.Domain;

/// <summary>
/// Notification entity for user notifications
/// </summary>
public class Notification : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid RecipientUserId { get; private set; }
    public string Title { get; private set; }
    public string TitleAr { get; private set; }
    public string Message { get; private set; }
    public string MessageAr { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime? SentAt { get; private set; }
    public string ActionUrl { get; private set; }
    public string ActionType { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public string RelatedEntityType { get; private set; }
    public string Icon { get; private set; }
    public string Color { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string Metadata { get; private set; }

    protected Notification() { }

    public Notification(
        Guid id,
        Guid recipientUserId,
        string title,
        string message,
        NotificationType type,
        NotificationChannel channel = NotificationChannel.InApp,
        Guid? tenantId = null)
        : base(id)
    {
        RecipientUserId = recipientUserId;
        Title = title;
        Message = message;
        Type = type;
        Channel = channel;
        Priority = NotificationPriority.Normal;
        IsRead = false;
        IsSent = false;
        TenantId = tenantId;
    }

    public void SetArabicContent(string titleAr, string messageAr)
    {
        TitleAr = titleAr;
        MessageAr = messageAr;
    }

    public void SetPriority(NotificationPriority priority)
    {
        Priority = priority;
    }

    public void SetAction(string actionUrl, string actionType)
    {
        ActionUrl = actionUrl;
        ActionType = actionType;
    }

    public void LinkToEntity(Guid entityId, string entityType)
    {
        RelatedEntityId = entityId;
        RelatedEntityType = entityType;
    }

    public void SetAppearance(string icon, string color)
    {
        Icon = icon;
        Color = color;
    }

    public void SetExpiry(DateTime expiresAt)
    {
        ExpiresAt = expiresAt;
    }

    public void SetMetadata(string metadata)
    {
        Metadata = metadata;
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
    }

    public void MarkAsSent()
    {
        if (!IsSent)
        {
            IsSent = true;
            SentAt = DateTime.UtcNow;
        }
    }
}

public enum NotificationType
{
    Info = 0,
    Warning = 1,
    Success = 2,
    Error = 3,
    Alert = 4,
    Reminder = 5,
    Assignment = 6,
    Approval = 7,
    Deadline = 8,
    Update = 9
}

public enum NotificationPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}

public enum NotificationChannel
{
    InApp = 0,
    Email = 1,
    SMS = 2,
    Push = 3,
    All = 99
}
