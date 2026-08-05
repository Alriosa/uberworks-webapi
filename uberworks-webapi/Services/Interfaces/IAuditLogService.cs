// =====================================================================================
// FILE SUMMARY
// What it does: Contract for writing to the 3 audit log tables. Every other Service (or
// the error middleware) calls this instead of touching IAuditLogRepository directly, so
// there's one place that fills in Source/IpAddress automatically and guarantees a logging
// failure never breaks the real request.
// Entities connected: ErrorLog.cs, UserActionLog.cs, AdminActionLog.cs
// Tables related: TBL_ERROR_LOGS, TBL_USER_ACTION_LOGS, TBL_ADMIN_ACTION_LOGS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Services.Interfaces;

public interface IAuditLogService
{
    /// <summary>Logs an action a user took on their own account (register, login, self-update).</summary>
    Task LogUserActionAsync(
        int? actorUserId,
        string? actorUsername,
        string action,
        string? targetEntityType = null,
        int? targetEntityId = null,
        string? details = null);

    /// <summary>Logs an action an Admin/MasterAdmin took on someone else's account or data.</summary>
    Task LogAdminActionAsync(
        int actorUserId,
        string actorUsername,
        UserRole actorRole,
        string action,
        string? targetEntityType = null,
        int? targetEntityId = null,
        string? details = null);

    /// <summary>Logs an unhandled/unexpected error (HTTP 500). Called from
    /// Middleware/ExceptionHandlingMiddleware.cs for the whole app.</summary>
    Task LogErrorAsync(Exception exception, string requestMethod, string requestPath, int statusCode);
}
