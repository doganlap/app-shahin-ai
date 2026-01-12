using System.Threading.Tasks;
using Grc.Events;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Grc.Product.Application.EventHandlers;

/// <summary>
/// Event handler for subscription cancellation
/// </summary>
public class SubscriptionCancelledEventHandler : 
    IDistributedEventHandler<SubscriptionCancelledEto>,
    ITransientDependency
{
    private readonly ILogger<SubscriptionCancelledEventHandler> _logger;

    public SubscriptionCancelledEventHandler(ILogger<SubscriptionCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(SubscriptionCancelledEto eventData)
    {
        _logger.LogInformation("Subscription cancelled: {SubscriptionId} for tenant {TenantId}. Reason: {Reason}", 
            eventData.SubscriptionId, eventData.TenantId, eventData.Reason);
        
        // TODO: Send cancellation email
        // TODO: Schedule data retention period
        // TODO: Notify admins
        
        await Task.CompletedTask;
    }
}

