// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/AuthResponse.cs — what
//               POST /api/users/login (and /login, /external-login) returns: the user's
//               public data, the JWT the WebApp must store to call protected endpoints later
//               (see Controllers/AccountController.cs, which stores it in an auth cookie),
//               and RequiresPasswordSetup — true only for a Google/Facebook-created account
//               that hasn't set a real password yet, which is what tells AppClaimsFactory.cs
//               to add the "requires_password_setup" claim that drives the "create your
//               password" modal (see _SetPasswordModal.cshtml).
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class AuthResponse
{
    public UserResponse User { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool RequiresPasswordSetup { get; set; }
}
