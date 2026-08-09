// =====================================================================================
// FILE SUMMARY
// What it does: Defines the possible states of a user account (active, suspended,
//               penalized, deleted). Used to block someone's access without physically
//               deleting their row (e.g. if an Admin suspends them for misconduct, or
//               "deletes" their account from the Admin dashboard) — a real hard DELETE would
//               either fail on foreign keys (Professional profile, Services requested,
//               Reviews, Chats, Penalties, PasswordResetTokens all reference TBL_USERS) or
//               silently orphan/erase real history. Deleted is appended at the END, same
//               reasoning as UserRole.cs's Manager/Company/Support: enums serialize as plain
//               integers and uberworks-webapp keeps its own copy of this exact enum.
// Entities connected: User.cs (the User.Status property is of this type)
// Tables related: TBL_USERS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the values used in TBL_USERS.CL_STATUS. See UserService.DeleteAsync/LoginAsync
/// for how Deleted (and Suspended) actually block someone out — this enum value alone does
/// nothing by itself.
/// </summary>
public enum UserStatus
{
    Active,
    Suspended,
    Penalized,
    Deleted
}
