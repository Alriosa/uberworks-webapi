// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Excepción personalizada. Cuando un Service (capa de negocio) busca algo por id
//           y no lo encuentra (ej. "GetByIdAsync(999)"), en vez de devolver null y que cada
//           Controller tenga que acordarse de chequearlo, se lanza esta excepción. El
//           Middleware/ExceptionHandlingMiddleware.cs la atrapa automáticamente y responde
//           HTTP 404 al cliente (webapp/mobile), sin que el Controller tenga que hacer nada.
// Entidades relacionadas: Ninguna directamente — la puede lanzar cualquier Service
//                          (UserService, ProfessionalService, ServiceService, etc.)
// Tablas relacionadas: Ninguna (es una clase de C#, no toca la base de datos)
// =====================================================================================
namespace uberworks_webapi.Common.Exceptions;

/// <summary>
/// Se lanza cuando un recurso solicitado no existe. El middleware la traduce a HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
