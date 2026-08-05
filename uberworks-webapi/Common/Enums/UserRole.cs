// =====================================================================================
// FILE SUMMARY
// What it does: Defines the 4 possible roles a user can have in the system. C# doesn't
//               allow storing "safe" free text in a property, so instead of a loose string
//               (where someone could write "cLiEnT" and break things), an enum is used: a
//               closed list of valid options the compiler knows and validates.
// Entities connected: User.cs (the User.Role property is of this type)
// Tables related: TBL_USERS.CL_ROLE (the C# value is translated to text in
//                 Data/Configurations/UserConfiguration.cs before being saved)
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the CHECK constraint on TBL_USERS.CL_ROLE.
///
/// MasterAdmin: a single account, seeded directly into the database on API startup
/// (see Data/Seed/MasterAdminSeeder.cs) — it can never be created via /api/users/register.
/// Admin: regular administrators with permissions delegated by the MasterAdmin
/// (create/delete accounts, global notifications, etc. — to be defined later).
/// Also cannot be created via the public /register endpoint.
/// </summary>
public enum UserRole
{
    MasterAdmin,
    Admin,
    Client,
    Professional
}
