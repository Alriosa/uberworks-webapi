// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Excepción personalizada específica para login fallido (email no existe o
//           password incorrecto). Se lanza desde UserService.LoginAsync() y el
//           Middleware/ExceptionHandlingMiddleware.cs la convierte en HTTP 401 Unauthorized.
// Entidades relacionadas: User.cs (indirectamente, vía UserService.LoginAsync)
// Tablas relacionadas: Ninguna (es una clase de C#, no toca la base de datos)
// =====================================================================================
namespace uberworks_webapi.Common.Exceptions;

/// <summary>
/// Se lanza cuando el email o la contraseña de login no son válidos.
/// El middleware la traduce a HTTP 401.
/// </summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message) : base(message)
    {
    }
}
