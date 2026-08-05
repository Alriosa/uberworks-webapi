// =====================================================================================
// FILE SUMMARY
// What it does: Describes exactly what fields the client (webapp/mobile) must send in the
//               body of POST /api/users/register. It's a "transport" class (DTO = Data
//               Transfer Object) — never saved directly to the database; UserService.cs
//               reads it and builds a User.cs from this data (plus the hashed password).
// Entities connected: User.cs (UserService.RegisterAsync converts this into a User)
// Tables related: None directly — only reaches TBL_USERS after passing through UserService.cs
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Requests;

public class RegisterUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
