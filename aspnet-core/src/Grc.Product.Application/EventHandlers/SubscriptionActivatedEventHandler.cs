using System.Threading.Tasks;
using Grc.Events;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Grc.Product.Application.EventHandlers;

/// <summary>
/// Event handler for subscription activation
/// </summary>
public class SubscriptionActivatedEventHandler : 
    IDistributedEventHandler<SubscriptionActivatedEto>,
    ITransientDependency
{
    private readonly ILogger<SubscriptionActivatedEventHandler> _logger;

    public SubscriptionActivatedEventHandler(ILogger<SubscriptionActivatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(SubscriptionActivatedEto eventData)
    {
        _logger.LogInformation("Subscription activated: {SubscriptionId} for tenant {TenantId}", 
            eventData.SubscriptionId, eventData.TenantId);
        
        // TODO: Send welcome email
        // TODO: Initialize quota usages
        // TODO: Update tenant configuration
        
        await Task.CompletedTask;
    }
}

