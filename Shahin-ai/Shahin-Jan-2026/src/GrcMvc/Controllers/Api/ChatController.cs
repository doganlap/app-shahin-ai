using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Threading;
using System.Threading.Tasks;
using GrcMvc.Controllers.Api;
using GrcMvc.Filters;
using GrcMvc.Models.DTOs;
using GrcMvc.Services.Interfaces;

namespace GrcMvc.Controllers.Api
{
    /// <summary>
    /// API Controller for landing page chat functionality
    /// Supports both anonymous and authenticated users
    /// </summary>
    [ApiController]
    [Route("api/v1/chat")]
    [ServiceFilter(typeof(ApiExceptionFilterAttribute))]
    public class ChatController : GrcApiControllerBase
    {
        private readonly ILandingChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            ILandingChatService chatService,
            ILogger<ChatController> logger)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Create a new chat session (anonymous or authenticated)
        /// POST /api/v1/chat/sessions
        /// </summary>
        [HttpPost("sessions")]
        [AllowAnonymous]
        [EnableRateLimiting("chat-anonymous")]
        public async Task<IActionResult> CreateSession(
            [FromBody] CreateChatSessionInputDto input,
            CancellationToken cancellationToken = default)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _chatService.CreateSessionAsync(
                input,
                ipAddress,
                userAgent,
                cancellationToken);

            return FromResult(result);
        }

        /// <summary>
        /// Send a chat message and get AI response
        /// POST /api/v1/chat/messages
        /// </summary>
        [HttpPost("messages")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken] // CSRF protection
        public async Task<IActionResult> SendMessage(
            [FromBody] SendChatMessageInputDto input,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            Guid? tenantId = null;

            if (isAuthenticated && Guid.TryParse(User.FindFirst("TenantId")?.Value, out var tid))
            {
                tenantId = tid;
            }

            var result = await _chatService.SendMessageAsync(
                input,
                userId,
                tenantId,
                ipAddress,
                cancellationToken);

            return FromResult(result);
        }

        /// <summary>
        /// Get conversation history by session ID
        /// GET /api/v1/chat/sessions/{sessionId}/messages
        /// </summary>
        [HttpGet("sessions/{sessionId}/messages")]
        [AllowAnonymous]
        [EnableRateLimiting("chat-anonymous")]
        public async Task<IActionResult> GetConversationHistory(
            string sessionId,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var result = await _chatService.GetConversationHistoryAsync(
                sessionId,
                userId,
                cancellationToken);

            return FromResult(result);
        }

        /// <summary>
        /// Escalate conversation to human support
        /// POST /api/v1/chat/escalate
        /// </summary>
        [HttpPost("escalate")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("chat-anonymous")]
        public async Task<IActionResult> EscalateToSupport(
            [FromBody] EscalateChatInputDto input,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            Guid? tenantId = null;

            if (User.Identity?.IsAuthenticated == true &&
                Guid.TryParse(User.FindFirst("TenantId")?.Value, out var tid))
            {
                tenantId = tid;
            }

            var result = await _chatService.EscalateToSupportAsync(
                input,
                userId,
                tenantId,
                cancellationToken);

            return FromResult(result);
        }

        /// <summary>
        /// Resolve/close a conversation
        /// POST /api/v1/chat/resolve
        /// </summary>
        [HttpPost("resolve")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("chat-anonymous")]
        public async Task<IActionResult> ResolveConversation(
            [FromBody] ResolveChatInputDto input,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var result = await _chatService.ResolveConversationAsync(
                input,
                userId,
                cancellationToken);

            if (result.IsSuccess)
                return Success("المحادثة تم إغلاقها بنجاح | Conversation resolved successfully");

            return FromResult(result);
        }
    }
}
