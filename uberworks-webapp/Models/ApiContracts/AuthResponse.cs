// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/AuthResponse.cs — what
//               POST /api/users/login returns: the user's public data plus the JWT the
//               WebApp must store to call protected endpoints later (see
//               Controllers/AccountController.cs, which stores it in an auth cookie).
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class AuthResponse
{
    public UserResponse User { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
