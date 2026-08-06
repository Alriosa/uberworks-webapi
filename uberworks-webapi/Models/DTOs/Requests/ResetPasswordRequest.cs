// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/users/reset-password. Token is the raw
//               value from the email link (hashed and looked up by
//               UserService.ResetPasswordAsync, via IPasswordResetTokenRepository) —
//               never the token's hash, which only ever lives in the database.
// Entities connected: None directly
// Tables related: None directly
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
