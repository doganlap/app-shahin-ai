using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Notification.Domain;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace Grc.Notification.Application;

/// <summary>
/// Multi-channel notification service
/// </summary>
public class NotificationService
{
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly IEmailChannel _emailChannel;
    private readonly ISmsChannel _smsChannel;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IRepository<Notification, Guid> notificationRepository,
        IEmailChannel emailChannel,
        ISmsChannel smsChannel,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _emailChannel = emailChannel;
        _smsChannel = smsChannel;
        _logger = logger;
    }

    /// <summary>
    /// Send notification through specified channels
    /// </summary>
    public async Task SendNotificationAsync(
        string recipientUserId,
        string title,
        string message,
        string notificationType,
        List<string> channels,
        Guid? tenantId = null,
        Dictionary<string, object> metadata = null)
    {
        foreach (var channel in channels)
        {
            try
            {
                var notification = new Notification(
                    Guid.NewGuid(),
                    recipientUserId,
                    title,
                    message,
                    notificationType,
                    channel)
                {
                    TenantId = tenantId,
                    Metadata = metadata ?? new Dictionary<string, object>()
                };

                // Send through appropriate channel
                switch (channel.ToLower())
                {
                    case "email":
                        await _emailChannel.SendAsync(recipientUserId, title, message);
                        break;
                    case "sms":
                        await _smsChannel.SendAsync(recipientUserId, message);
                        break;
                    case "inapp":
                        // In-app notifications are stored in database
                        break;
                }

                await _notificationRepository.InsertAsync(notification);
                _logger.LogInformation("Sent {Channel} notification to user {UserId}", channel, recipientUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending {Channel} notification to user {UserId}", channel, recipientUserId);
            }
        }
    }

    /// <summary>
    /// Get unread notifications for user
    /// </summary>
    public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
    {
        return await _notificationRepository.GetListAsync(n => 
            n.RecipientUserId == userId && 
            !n.IsRead,
            orderBy: n => n.SentAt,
            descending: true);
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _notificationRepository.GetAsync(notificationId);
        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification);
    }
}

