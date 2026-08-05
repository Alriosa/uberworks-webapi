// =====================================================================================
// FILE SUMMARY
// What it does: Contract for asking "who is making this request right now, and how?" from
//               any Controller or Service, without having to read HttpContext directly
//               everywhere. UserId/Username/Role come from the JWT already validated by
//               ASP.NET Core before it reaches the Controller. Source and IpAddress feed
//               the audit log system (Services/AuditLogService.cs) so every log row can
//               record not just who did something, but also how (WebApp/MobileApp/Direct)
//               and from where.
// Entities connected: User.cs (UserId/Username/Role reflect the authenticated User's)
// Tables related: None (reads from the in-memory token/request, not the database)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Services.Interfaces;

/// <summary>
/// Exposes the authenticated user's identity (extracted from the JWT) plus request
/// metadata (client source, IP) to Services/Controllers, without coupling them directly
/// to HttpContext.
/// </summary>
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    int? UserId { get; }
    string? Username { get; }
    UserRole? Role { get; }

    /// <summary>Which client made the call (WebApp/MobileApp/Direct), read from the
    /// "X-Client-Source" header. Used exclusively for audit logging.</summary>
    LogSource Source { get; }

    /// <summary>Caller's IP address, or null if unavailable. Used exclusively for audit logging.</summary>
    string? IpAddress { get; }
}
