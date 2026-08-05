// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/users/login (just email + password). It's
//               an input DTO — UserService.LoginAsync uses it to find the User by email and
//               verify the password with PasswordHasher.Verify(), never seeing the real hash.
// Entities connected: User.cs (indirectly, via UserService.LoginAsync)
// Tables related: None directly (TBL_USERS is queried from UserService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
