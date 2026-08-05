// =====================================================================================
// FILE SUMMARY
// What it does: Contract for asking "who is making this request right now?" from any
//               Controller or Service, without having to read HttpContext directly
//               everywhere. It gets populated from the JWT already validated by ASP.NET
//               Core before it reaches the Controller.
// Entities connected: User.cs (UserId/Role reflect the authenticated User's)
// Tables related: None (reads from the in-memory token, not the database)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Services.Interfaces;

/// <summary>
/// Exposes the authenticated user's identity (extracted from the JWT) to
/// Services/Controllers, without coupling them directly to HttpContext.
/// </summary>
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    int? UserId { get; }
    UserRole? Role { get; }
}
