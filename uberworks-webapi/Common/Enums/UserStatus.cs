// =====================================================================================
// FILE SUMMARY
// What it does: Defines the possible states of a user account (active, suspended,
//               penalized). Used to block someone's access without deleting their account
//               or history (e.g. if an Admin suspends them for misconduct).
// Entities connected: User.cs (the User.Status property is of this type)
// Tables related: TBL_USERS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the values used in TBL_USERS.CL_STATUS.
/// </summary>
public enum UserStatus
{
    Active,
    Suspended,
    Penalized
}
