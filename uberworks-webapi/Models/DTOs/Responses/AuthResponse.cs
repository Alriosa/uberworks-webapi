// =====================================================================================
// FILE SUMMARY
// What it does: This is what POST /api/users/login (and /login, /external-login) returns —
//               the user's public data (UserResponse), the JWT token the client (webapp/
//               mobile) must store, when it expires, and RequiresPasswordSetup — true only
//               for accounts that were auto-created via Google/Facebook and haven't set a
//               real password yet (User.IsPasswordSet == false). The WebApp uses this flag to
//               decide whether to show the "create your password" modal after signing this
//               person in — see AccountController/Program.cs's OAuth callbacks.
// Entities connected: User.cs (indirectly, via UserResponse)
// Tables related: None (the Token isn't stored in the database, it's just signed and returned)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class AuthResponse
{
    public UserResponse User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool RequiresPasswordSetup { get; set; }
}
