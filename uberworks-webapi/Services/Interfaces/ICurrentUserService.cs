// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato para preguntar "¿quién está haciendo esta petición ahora mismo?"
//           desde cualquier Controller o Service, sin tener que leer HttpContext
//           directamente en todos lados. Se llena a partir del JWT ya validado por
//           ASP.NET Core antes de llegar al Controller.
// Entidades relacionadas: User.cs (UserId/Role reflejan los del User autenticado)
// Tablas relacionadas: Ninguna (lee del token en memoria, no de la base de datos)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Services.Interfaces;

/// <summary>
/// Expone la identidad del usuario autenticado (extraída del JWT) a Services/Controllers,
/// sin acoplarlos directamente a HttpContext.
/// </summary>
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    int? UserId { get; }
    UserRole? Role { get; }
}
