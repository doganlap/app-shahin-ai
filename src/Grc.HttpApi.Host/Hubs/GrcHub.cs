using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace Grc.Hubs;

/// <summary>
/// SignalR hub for real-time GRC platform updates
/// </summary>
public class GrcHub : AbpHub
{
    /// <summary>
    /// Join an assessment room to receive real-time updates
    /// </summary>
    public async Task JoinAssessmentRoom(Guid assessmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"assessment:{assessmentId}");
    }
    
    /// <summary>
    /// Leave an assessment room
    /// </summary>
    public async Task LeaveAssessmentRoom(Guid assessmentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"assessment:{assessmentId}");
    }
    
    /// <summary>
    /// Join a control assessment room
    /// </summary>
    public async Task JoinControlAssessmentRoom(Guid controlAssessmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"control-assessment:{controlAssessmentId}");
    }
    
    /// <summary>
    /// Leave a control assessment room
    /// </summary>
    public async Task LeaveControlAssessmentRoom(Guid controlAssessmentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"control-assessment:{controlAssessmentId}");
    }
    
    /// <summary>
    /// Notify that a control has been updated
    /// </summary>
    public async Task NotifyControlUpdated(Guid controlAssessmentId, object updateData)
    {
        await Clients.Group($"control-assessment:{controlAssessmentId}")
            .SendAsync("ControlUpdated", controlAssessmentId, updateData);
    }
    
    /// <summary>
    /// Notify that assessment progress has changed
    /// </summary>
    public async Task NotifyAssessmentProgress(Guid assessmentId, object progressData)
    {
        await Clients.Group($"assessment:{assessmentId}")
            .SendAsync("AssessmentProgress", assessmentId, progressData);
    }
    
    /// <summary>
    /// Notify that a user is typing in a control assessment
    /// </summary>
    public async Task NotifyUserTyping(Guid controlAssessmentId, string userId, string userName)
    {
        await Clients.GroupExcept($"control-assessment:{controlAssessmentId}", Context.ConnectionId)
            .SendAsync("UserTyping", controlAssessmentId, userId, userName);
    }
    
    /// <summary>
    /// Send notification to a specific user
    /// </summary>
    public async Task SendNotification(string userId, object notification)
    {
        await Clients.User(userId).SendAsync("Notification", notification);
    }
    
    /// <summary>
    /// Send notification to all users in a tenant
    /// </summary>
    public async Task SendTenantNotification(string tenantId, object notification)
    {
        await Clients.Group($"tenant:{tenantId}").SendAsync("TenantNotification", notification);
    }
    
    public override async Task OnConnectedAsync()
    {
        // Add user to tenant group if tenant is available
        var tenantId = Context.User?.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}");
        }
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var tenantId = Context.User?.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant:{tenantId}");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}

