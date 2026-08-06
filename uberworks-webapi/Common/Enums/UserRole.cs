// =====================================================================================
// FILE SUMMARY
// What it does: Defines the 6 possible roles a user can have in the system. C# doesn't
//               allow storing "safe" free text in a property, so instead of a loose string
//               (where someone could write "cLiEnT" and break things), an enum is used: a
//               closed list of valid options the compiler knows and validates. Manager and
//               Company were appended at the END on purpose, never inserted between existing
//               values: System.Text.Json serializes enums as plain integers by default, and
//               uberworks-webapp keeps its own copy of this exact enum (Models/ApiContracts/
//               UserRole.cs) — reordering existing members would silently misread every
//               Role value already flowing between the two projects.
// Entities connected: User.cs (the User.Role property is of this type)
// Tables related: TBL_USERS.CL_ROLE (the C# value is translated to text in
//                 Data/Configurations/UserConfiguration.cs before being saved)
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the CHECK constraint on TBL_USERS.CL_ROLE.
///
/// Account-creation pyramid (see UserService.CreateByAdminAsync for the enforced rules):
/// MasterAdmin creates Admin/Manager/Company/Professional/Client. Admin creates
/// Manager/Company/Professional/Client. Manager creates Company/Professional/Client.
/// Client is the only role that can also self-register (POST /api/users/register).
///
/// MasterAdmin: a single account, seeded directly into the database on API startup
/// (see Data/Seed/MasterAdminSeeder.cs) — it can never be created via /api/users/register
/// or via /api/users/admin-create.
/// Admin: regular administrators, created only by MasterAdmin. Can create Manager,
/// Company, Professional, and Client accounts.
/// Manager: created by Admin/MasterAdmin. Can create Company, Professional, and Client
/// accounts — this is the role responsible for onboarding workers and companies.
/// Company: a business account (e.g. an events company) that can create Professional
/// ("worker") accounts of its own, which stay linked to it via
/// Professional.CompanyUserId — see POST /api/professionals/company-create.
/// Client: anyone requesting services. The only role that can self-register.
/// Professional: a worker offering services, either self-registered... no — never
/// self-registered (see RegisterAsync, which only accepts Client); always created by
/// Manager/Admin/MasterAdmin (as a plain worker) or by a Company (as one of its workers).
/// </summary>
public enum UserRole
{
    MasterAdmin,
    Admin,
    Client,
    Professional,
    Manager,
    Company
}
