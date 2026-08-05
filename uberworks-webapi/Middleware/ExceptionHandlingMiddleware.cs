// =====================================================================================
// FILE SUMMARY
// What it does: This is a "layer" (middleware) that wraps EVERY HTTP request coming into
//               the API. If any Controller/Service throws one of the custom exceptions from
//               Common/Exceptions (NotFoundException, ConflictException, etc.), this code
//               catches it in a single place and builds the right HTTP response
//               (404/409/401/400), instead of having to repeat a try/catch in each of the
//               ~15 Controller methods. Any unrecognized exception becomes a 500 and gets
//               logged (the internal detail is never shown to the client, for security).
// Entities connected: None directly — this is cross-cutting infrastructure
// Tables related: None (doesn't touch the database)
// =====================================================================================
using System.Net;
using System.Text.Json;
using uberworks_webapi.Common.Exceptions;

namespace uberworks_webapi.Middleware;

/// <summary>
/// Catches the domain exceptions (Common/Exceptions) and any unhandled exception,
/// always returning a consistent JSON error across the whole API.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            InvalidCredentialsException => HttpStatusCode.Unauthorized,
            ArgumentException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var message = statusCode == HttpStatusCode.InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;

        var payload = JsonSerializer.Serialize(new { statusCode = (int)statusCode, message });
        await context.Response.WriteAsync(payload);
    }
}
