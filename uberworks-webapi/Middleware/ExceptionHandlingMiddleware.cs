// =====================================================================================
// FILE SUMMARY
// What it does: This is a "layer" (middleware) that wraps EVERY HTTP request coming into
//               the API. If any Controller/Service throws one of the custom exceptions from
//               Common/Exceptions (NotFoundException, ConflictException, etc.), this code
//               catches it in a single place and builds the right HTTP response
//               (404/409/403/401/400), instead of having to repeat a try/catch in each of
//               the Controller methods. Any unrecognized exception becomes a 500, gets
//               logged to the normal application logger (console/file) AND written to
//               TBL_ERROR_LOGS via IAuditLogService — this is what gives "TODA LA APP" (the
//               whole app) automatic error logging without touching every Controller
//               individually. The internal detail (stack trace) is never shown to the
//               client, for security — it only goes to the log.
// Entities connected: ErrorLog.cs (indirectly, via IAuditLogService)
// Tables related: TBL_ERROR_LOGS (indirectly)
// =====================================================================================
using System.Net;
using System.Text.Json;
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Services.Interfaces;

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

    // IAuditLogService is injected here as a method parameter (not through the constructor)
    // because middleware is built once at startup, while IAuditLogService is a scoped
    // service tied to a single request — ASP.NET Core resolves method parameters like this
    // fresh on every call to InvokeAsync.
    public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, auditLogService);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, IAuditLogService auditLogService)
    {
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            ForbiddenException => HttpStatusCode.Forbidden,
            InvalidCredentialsException => HttpStatusCode.Unauthorized,
            ArgumentException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");

            await auditLogService.LogErrorAsync(
                exception,
                context.Request.Method,
                context.Request.Path,
                (int)statusCode);
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
