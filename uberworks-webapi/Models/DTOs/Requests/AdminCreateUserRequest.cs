// =====================================================================================
// FILE SUMMARY
// What it does: Describes exactly what fields an authenticated Manager/Admin/MasterAdmin
//               must send in the body of POST /api/users/admin-create. Unlike
//               RegisterUserRequest.cs (the public endpoint), Role here can be anything
//               except MasterAdmin (there must only ever be one, seeded automatically on
//               startup — see Data/Seed/MasterAdminSeeder.cs) — WHICH roles a given caller
//               is actually allowed to pick depends on their own role and is enforced by
//               UserService.CreateByAdminAsync's CreatableRolesByActor table (the
//               account-creation pyramid documented on UserRole.cs), not by this DTO. It's
//               a "transport" class (DTO = Data Transfer Object) — never saved directly to
//               the database; UserService.CreateByAdminAsync reads it and builds a User.cs
//               from this data (plus the hashed password).
// Entities connected: User.cs (UserService.CreateByAdminAsync converts this into a User)
// Tables related: None directly — only reaches TBL_USERS after passing through UserService.cs
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Requests;

public class AdminCreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
