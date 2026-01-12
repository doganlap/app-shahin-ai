using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Grc.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        context.Response.Headers["X-Request-ID"] = requestId;

        // Log request
        _logger.LogInformation(
            "HTTP {Method} {Path} started. RequestId: {RequestId}, IP: {IP}, UserAgent: {UserAgent}",
            context.Request.Method,
            context.Request.Path,
            requestId,
            context.Connection.RemoteIpAddress?.ToString(),
            context.Request.Headers["User-Agent"].ToString());

        // Capture original response body stream
        var originalBodyStream = context.Response.Body;

        try
        {
            await _next(context);

            stopwatch.Stop();

            // Log response
            _logger.LogInformation(
                "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms. RequestId: {RequestId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                requestId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "HTTP {Method} {Path} failed with exception after {ElapsedMs}ms. RequestId: {RequestId}, Error: {ErrorMessage}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                requestId,
                ex.Message);

            throw;
        }
    }
}
