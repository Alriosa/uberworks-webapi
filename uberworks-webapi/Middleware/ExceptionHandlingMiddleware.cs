// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es una "capa" (middleware) que envuelve TODA petición HTTP que entra a la API.
//           Si algún Controller/Service lanza una de las excepciones custom de
//           Common/Exceptions (NotFoundException, ConflictException, etc.), este código la
//           atrapa aquí en un solo lugar y arma la respuesta HTTP correcta (404/409/401/400),
//           en vez de tener que repetir un try/catch en cada uno de los ~15 métodos de
//           Controller. Cualquier excepción no reconocida se convierte en 500 y se registra
//           en el log (nunca se le muestra el detalle interno al cliente, por seguridad).
// Entidades relacionadas: Ninguna directamente — es infraestructura transversal
// Tablas relacionadas: Ninguna (no toca la base de datos)
// =====================================================================================
using System.Net;
using System.Text.Json;
using uberworks_webapi.Common.Exceptions;

namespace uberworks_webapi.Middleware;

/// <summary>
/// Captura las excepciones de dominio (Common/Exceptions) y cualquier excepción no
/// controlada, devolviendo siempre un JSON de error consistente en toda la API.
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
            _logger.LogError(exception, "Excepción no controlada");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var message = statusCode == HttpStatusCode.InternalServerError
            ? "Ocurrió un error inesperado."
            : exception.Message;

        var payload = JsonSerializer.Serialize(new { statusCode = (int)statusCode, message });
        await context.Response.WriteAsync(payload);
    }
}
