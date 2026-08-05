// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of ICurrentUserService.cs. Reads the "claims" (the
//               data JwtTokenService.cs put inside the token at login) from
//               HttpContext.User — which ASP.NET Core fills in automatically after
//               validating the JWT's signature on every incoming request (see Program.cs →
//               UseAuthentication()). If there's no valid token, every property returns
//               null/false.
// Entities connected: User.cs (indirectly, via the token's claims)
// Tables related: None (reads from the in-memory token, not the database)
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
