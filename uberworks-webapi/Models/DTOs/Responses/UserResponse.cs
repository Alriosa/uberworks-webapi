// =====================================================================================
// FILE SUMMARY
// What it does: Describes what the API returns when querying a user. Intentionally does
//               NOT include PasswordHash — it should never be exposed, not even the hash,
//               in an HTTP response. UserService.cs builds this object from a real User.cs.
// Entities connected: User.cs (UserService.cs maps from one to the other)
// Tables related: None directly — it's the "public shape" of a TBL_USERS row
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public DateTime RegistrationDate { get; set; }
}
