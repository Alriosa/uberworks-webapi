// =====================================================================================
// FILE SUMMARY
// What it does: One row per action an Admin/MasterAdmin takes on SOMEONE ELSE's account or
//               data (e.g. editing another user's profile, and — once built — creating
//               Admin accounts, applying penalties, sending global notifications, etc.).
//               Kept as a separate table from UserActionLog.cs on purpose, per your request
//               to have administrator actions fully separated for auditing.
// Entities connected: None (intentionally has no FK to User — snapshot fields survive even
//                      if the account is later deleted, for audit purposes)
// Tables related: TBL_ADMIN_ACTION_LOGS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

public class AdminActionLog
{
    public int Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public LogSource Source { get; set; }

    public int ActorUserId { get; set; }
    public string ActorUsername { get; set; } = string.Empty;
    public UserRole ActorRole { get; set; }

    /// <summary>Short machine-readable event name, e.g. "USER_PROFILE_UPDATED_BY_ADMIN".</summary>
    public string Action { get; set; } = string.Empty;

    public string? TargetEntityType { get; set; }
    public int? TargetEntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
}
