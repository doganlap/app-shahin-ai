using Grc.Hubs;
using HotChocolate;
using Microsoft.AspNetCore.SignalR;

namespace Grc.GraphQL;

/// <summary>
/// GraphQL Subscription root for real-time updates
/// </summary>
public class Subscription
{
    [Subscribe]
    public async Task<ControlAssessmentDto> OnControlUpdated(
        [Service] IHubContext<GrcHub> hubContext)
    {
        // TODO: Implement SignalR subscription
        return null; // Placeholder
    }

    [Subscribe]
    public async Task<AssessmentProgressDto> OnAssessmentProgress(
        System.Guid assessmentId,
        [Service] IHubContext<GrcHub> hubContext)
    {
        // TODO: Implement SignalR subscription
        return null; // Placeholder
    }
}

