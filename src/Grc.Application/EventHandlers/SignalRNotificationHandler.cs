using System.Threading.Tasks;
using Grc.Events;
using Grc.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Grc.Application.EventHandlers;

/// <summary>
/// SignalR notification handler for domain events
/// </summary>
public class SignalRNotificationHandler : 
    IDistributedEventHandler<ControlAssignedEto>,
    IDistributedEventHandler<ControlVerifiedEto>,
    IDistributedEventHandler<AssessmentProgressDto>,
    ITransientDependency
{
    private readonly IHubContext<GrcHub> _hubContext;
    private readonly ILogger<SignalRNotificationHandler> _logger;

    public SignalRNotificationHandler(
        IHubContext<GrcHub> hubContext,
        ILogger<SignalRNotificationHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task HandleEventAsync(ControlAssignedEto eventData)
    {
        _logger.LogDebug("Sending SignalR notification for control assignment: {ControlAssessmentId}", 
            eventData.ControlAssessmentId);
        
        // Notify user about new assignment
        await _hubContext.Clients.User(eventData.UserId.ToString())
            .SendAsync("ControlAssigned", new
            {
                ControlAssessmentId = eventData.ControlAssessmentId,
                DueDate = eventData.DueDate
            });
        
        // Notify assessment room
        // Note: Would need to get assessment ID from control assessment
        // await _hubContext.Clients.Group($"assessment:{assessmentId}")
        //     .SendAsync("ControlUpdated", eventData);
    }

    public async Task HandleEventAsync(ControlVerifiedEto eventData)
    {
        _logger.LogDebug("Sending SignalR notification for control verification: {ControlAssessmentId}", 
            eventData.ControlAssessmentId);
        
        // Notify control assessment room
        await _hubContext.Clients.Group($"control-assessment:{eventData.ControlAssessmentId}")
            .SendAsync("ControlUpdated", new
            {
                ControlAssessmentId = eventData.ControlAssessmentId,
                Status = "Verified",
                Score = eventData.Score,
                VerifierId = eventData.VerifierId
            });
    }

    public async Task HandleEventAsync(AssessmentProgressDto eventData)
    {
        // This would be triggered by assessment progress updates
        // Implementation would send progress updates to assessment room
        await Task.CompletedTask;
    }
}

