using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GrcMvc.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time chat communication
    /// Supports typing indicators and live message delivery
    /// </summary>
    [AllowAnonymous] // Allow anonymous for landing page chat
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var sessionId = Context.GetHttpContext()?.Request.Query["sessionId"].ToString();
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(sessionId))
            {
                // Join session-specific group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionId}");
                _logger.LogInformation("Client {ConnectionId} connected to chat session {SessionId}",
                    Context.ConnectionId, sessionId);
            }

            // Join user-specific group for authenticated users
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var sessionId = Context.GetHttpContext()?.Request.Query["sessionId"].ToString();

            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected from chat session {SessionId} with error", sessionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected from chat session {SessionId}", sessionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Notify typing indicator
        /// </summary>
        public async Task NotifyTyping(string sessionId, bool isTyping)
        {
            await Clients.OthersInGroup($"chat_{sessionId}").SendAsync("UserTyping", new
            {
                IsTyping = isTyping,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Ping to keep connection alive
        /// </summary>
        public Task Ping()
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Service to push chat updates to connected clients
    /// </summary>
    public interface IChatHubService
    {
        Task NotifyNewMessageAsync(string sessionId, string senderType, string message);
        Task NotifyEscalationAsync(string sessionId, Guid supportConversationId);
        Task NotifyConversationClosedAsync(string sessionId);
    }

    /// <summary>
    /// Implementation of chat hub service
    /// </summary>
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatHubService> _logger;

        public ChatHubService(
            IHubContext<ChatHub> hubContext,
            ILogger<ChatHubService> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task NotifyNewMessageAsync(string sessionId, string senderType, string message)
        {
            var group = $"chat_{sessionId}";
            await _hubContext.Clients.Group(group).SendAsync("NewMessage", new
            {
                SenderType = senderType,
                Message = message,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogDebug("Pushed new message notification to session {SessionId}", sessionId);
        }

        public async Task NotifyEscalationAsync(string sessionId, Guid supportConversationId)
        {
            var group = $"chat_{sessionId}";
            await _hubContext.Clients.Group(group).SendAsync("ConversationEscalated", new
            {
                SupportConversationId = supportConversationId,
                Message = "تم تصعيد المحادثة إلى فريق الدعم | Conversation escalated to support team",
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Pushed escalation notification to session {SessionId}", sessionId);
        }

        public async Task NotifyConversationClosedAsync(string sessionId)
        {
            var group = $"chat_{sessionId}";
            await _hubContext.Clients.Group(group).SendAsync("ConversationClosed", new
            {
                Message = "تم إغلاق المحادثة | Conversation closed",
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Pushed closure notification to session {SessionId}", sessionId);
        }
    }
}
