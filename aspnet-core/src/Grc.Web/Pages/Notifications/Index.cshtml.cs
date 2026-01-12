using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using NotificationEntity = Grc.Notification.Domain.Notification;
using NotificationType = Grc.Notification.Domain.NotificationType;
using NotificationPriority = Grc.Notification.Domain.NotificationPriority;

namespace Grc.Web.Pages.Notifications;

[Authorize(GrcPermissions.Notifications.Default)]
public class IndexModel : GrcPageModel
{
    private readonly IRepository<NotificationEntity, Guid> _notificationRepository;

    public List<NotificationItem> Notifications { get; set; } = new();
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }
    public NotificationSummary Summary { get; set; } = new();

    public IndexModel(IRepository<NotificationEntity, Guid> notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task OnGetAsync()
    {
        var queryable = await _notificationRepository.GetQueryableAsync();

        // Get notifications from database, ordered by most recent first
        var notifications = await queryable
            .OrderByDescending(n => n.CreationTime)
            .Take(50)
            .ToListAsync();

        Notifications = notifications.Select(n => new NotificationItem
        {
            Id = n.Id,
            Title = n.Title,
            TitleAr = n.TitleAr,
            Message = n.Message,
            MessageAr = n.MessageAr,
            Type = GetTypeString(n.Type),
            TypeEnum = n.Type,
            Priority = n.Priority.ToString(),
            PriorityEnum = n.Priority,
            CreatedAt = n.CreationTime,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            ActionUrl = n.ActionUrl,
            ActionType = n.ActionType,
            Icon = n.Icon,
            Color = n.Color,
            RelatedEntityType = n.RelatedEntityType
        }).ToList();

        TotalCount = await queryable.CountAsync();
        UnreadCount = await queryable.CountAsync(n => !n.IsRead);

        // Calculate summary from actual data
        Summary = new NotificationSummary
        {
            Total = TotalCount,
            Unread = UnreadCount,
            Info = await queryable.CountAsync(n => n.Type == NotificationType.Info),
            Warning = await queryable.CountAsync(n => n.Type == NotificationType.Warning),
            Success = await queryable.CountAsync(n => n.Type == NotificationType.Success),
            Error = await queryable.CountAsync(n => n.Type == NotificationType.Error),
            HighPriority = await queryable.CountAsync(n => n.Priority >= NotificationPriority.High)
        };
    }

    private static string GetTypeString(NotificationType type)
    {
        return type switch
        {
            NotificationType.Info => "Info",
            NotificationType.Warning => "Warning",
            NotificationType.Success => "Success",
            NotificationType.Error => "Error",
            NotificationType.Alert => "Alert",
            NotificationType.Reminder => "Reminder",
            NotificationType.Assignment => "Assignment",
            NotificationType.Approval => "Approval",
            NotificationType.Deadline => "Deadline",
            NotificationType.Update => "Update",
            _ => "Info"
        };
    }
}

public class NotificationItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string MessageAr { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public NotificationType TypeEnum { get; set; }
    public string Priority { get; set; } = string.Empty;
    public NotificationPriority PriorityEnum { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string ActionUrl { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string RelatedEntityType { get; set; } = string.Empty;
}

public class NotificationSummary
{
    public int Total { get; set; }
    public int Unread { get; set; }
    public int Info { get; set; }
    public int Warning { get; set; }
    public int Success { get; set; }
    public int Error { get; set; }
    public int HighPriority { get; set; }
}
