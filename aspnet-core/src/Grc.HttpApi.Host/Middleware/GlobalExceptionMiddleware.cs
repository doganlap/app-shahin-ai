using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;

namespace Grc.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = Guid.NewGuid().ToString();
            _logger.LogError(ex, "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Message: {Message}",
                correlationId, context.Request.Path, ex.Message);
            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
    {
        var (code, errorCode, userMessage) = MapException(exception);

        var response = new
        {
            error = new
            {
                code = errorCode,
                message = userMessage,
                correlationId = correlationId,
                statusCode = (int)code,
                details = _environment.IsDevelopment() ? new
                {
                    type = exception.GetType().FullName,
                    stackTrace = exception.StackTrace,
                    innerException = exception.InnerException?.Message
                } : null
            }
        };

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result, Encoding.UTF8);
    }

    private static (HttpStatusCode code, string errorCode, string message) MapException(Exception exception)
    {
        return exception switch
        {
            // ABP Framework Exceptions
            AbpAuthorizationException => (HttpStatusCode.Forbidden, "FORBIDDEN", "You don't have permission to perform this action."),
            AbpValidationException validationEx => (HttpStatusCode.BadRequest, "VALIDATION_ERROR",
                $"Validation failed: {string.Join(", ", validationEx.ValidationErrors)}"),
            EntityNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND", "The requested resource was not found."),
            BusinessException businessEx => (HttpStatusCode.BadRequest, businessEx.Code ?? "BUSINESS_ERROR", businessEx.Message),

            // Security Exceptions
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED", "Authentication is required to access this resource."),
            SecurityException => (HttpStatusCode.Forbidden, "SECURITY_ERROR", "Access denied due to security restrictions."),

            // Argument/Input Exceptions
            ArgumentNullException argNull => (HttpStatusCode.BadRequest, "NULL_ARGUMENT", $"Required parameter '{argNull.ParamName}' was not provided."),
            ArgumentOutOfRangeException argRange => (HttpStatusCode.BadRequest, "OUT_OF_RANGE", $"Parameter '{argRange.ParamName}' is out of valid range."),
            ArgumentException argEx => (HttpStatusCode.BadRequest, "INVALID_ARGUMENT", argEx.Message),
            FormatException => (HttpStatusCode.BadRequest, "FORMAT_ERROR", "The provided data format is invalid."),

            // Data/Resource Exceptions
            KeyNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND", "The requested resource was not found."),
            FileNotFoundException => (HttpStatusCode.NotFound, "FILE_NOT_FOUND", "The requested file could not be found."),
            DirectoryNotFoundException => (HttpStatusCode.NotFound, "DIRECTORY_NOT_FOUND", "The specified directory could not be found."),

            // Concurrency/State Exceptions
            InvalidOperationException => (HttpStatusCode.Conflict, "INVALID_STATE", "The operation is not valid for the current state."),
            DBConcurrencyException => (HttpStatusCode.Conflict, "CONCURRENCY_ERROR", "The record was modified by another user. Please refresh and try again."),

            // Timeout Exceptions
            TimeoutException => (HttpStatusCode.GatewayTimeout, "TIMEOUT", "The operation timed out. Please try again."),
            TaskCanceledException => (HttpStatusCode.RequestTimeout, "REQUEST_CANCELLED", "The request was cancelled or timed out."),
            OperationCanceledException => (HttpStatusCode.RequestTimeout, "OPERATION_CANCELLED", "The operation was cancelled."),

            // Infrastructure Exceptions
            NotImplementedException => (HttpStatusCode.NotImplemented, "NOT_IMPLEMENTED", "This feature is not yet available."),
            NotSupportedException => (HttpStatusCode.BadRequest, "NOT_SUPPORTED", "This operation is not supported."),

            // Default case
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred. Please try again later.")
        };
    }
}
