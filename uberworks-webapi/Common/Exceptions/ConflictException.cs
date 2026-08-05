// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Excepción personalizada para cuando la operación pedida choca con el estado
//           actual de los datos (ej. registrar un email que ya existe, o intentar aceptar
//           una propuesta de un Service que ya no está pendiente). El
//           Middleware/ExceptionHandlingMiddleware.cs la convierte en HTTP 409 Conflict.
// Entidades relacionadas: Ninguna directamente — la lanzan varios Services
//                          (UserService, WorkTypeService, ServiceProfessionalService, etc.)
// Tablas relacionadas: Ninguna (es una clase de C#, no toca la base de datos)
// =====================================================================================
namespace uberworks_webapi.Common.Exceptions;

/// <summary>
/// Se lanza cuando la operación choca con el estado actual de los datos (ej. email duplicado).
/// El middleware la traduce a HTTP 409.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
