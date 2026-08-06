// =====================================================================================
// FILE SUMMARY
// What it does: Describes exactly what fields an authenticated Admin/MasterAdmin must send
//               in the body of POST /api/users/admin-create. Unlike RegisterUserRequest.cs
//               (the public endpoint), the Role here is allowed to be Admin — the only role
//               still forbidden is MasterAdmin, since there must only ever be one, seeded
//               automatically on startup (see Data/Seed/MasterAdminSeeder.cs). It's a
//               "transport" class (DTO = Data Transfer Object) — never saved directly to the
//               database; UserService.CreateByAdminAsync reads it and builds a User.cs from
//               this data (plus the hashed password).
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
