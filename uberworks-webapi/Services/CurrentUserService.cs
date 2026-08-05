// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Implementación real de ICurrentUserService.cs. Lee los "claims" (los datos que
//           JwtTokenService.cs metió dentro del token al hacer login) desde
//           HttpContext.User — que ASP.NET Core llena automáticamente después de validar
//           la firma del JWT en cada petición entrante (ver Program.cs → UseAuthentication()).
//           Si no hay token válido, todas las propiedades devuelven null/false.
// Entidades relacionadas: User.cs (indirectamente, vía los claims del token)
// Tablas relacionadas: Ninguna (lee del token en memoria, no de la base de datos)
// =====================================================================================
using System.Security.Claims;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public int? UserId
    {
        get
        {
            var value = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var value = Principal?.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRole>(value, ignoreCase: true, out var role) ? role : null;
        }
    }
}
