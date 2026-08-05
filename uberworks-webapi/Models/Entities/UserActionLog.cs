// =====================================================================================
// FILE SUMMARY
// What it does: One row per action a user takes on THEIR OWN account (register, login
//               success/failed, update their own profile). Written explicitly from
//               Services/UserService.cs (and, going forward, from any other Service that
//               handles a self-service write action) via IAuditLogService. Distinguished
//               from AdminActionLog.cs, which is for an Admin/MasterAdmin acting on someone
//               ELSE's account.
// Entities connected: None (intentionally has no FK to User — snapshot fields survive even
//                      if the account is later deleted, for audit purposes)
// Tables related: TBL_USER_ACTION_LOGS
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

public class UserActionLog
{
    public int Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public Common.Enums.LogSource Source { get; set; }

    /// <summary>
    /// Null only for a failed login attempt against an email that doesn't exist at all
    /// (there's no real account to attribute the attempt to).
    /// </summary>
    public int? ActorUserId { get; set; }
    public string? ActorUsername { get; set; }

    /// <summary>Short machine-readable event name, e.g. "USER_REGISTERED", "LOGIN_FAILED".</summary>
    public string Action { get; set; } = string.Empty;

    public string? TargetEntityType { get; set; }
    public int? TargetEntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
}
