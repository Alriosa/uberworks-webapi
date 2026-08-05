// =====================================================================================
// FILE SUMMARY
// What it does: This is what POST /api/users/login returns — the user's public data
//               (UserResponse), the JWT token the client (webapp/mobile) must store, and
//               when it expires (so the app knows when to request a new one).
// Entities connected: User.cs (indirectly, via UserResponse)
// Tables related: None (the Token isn't stored in the database, it's just signed and returned)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class AuthResponse
{
    public UserResponse User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
