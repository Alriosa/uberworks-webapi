// =====================================================================================
// FILE SUMMARY
// What it does: One row per "forgot password" request. The actual token that goes in the
//               email link is NEVER stored here — only its SHA256 hash (TokenHasher.cs),
//               same idea as User.PasswordHash: even a full database dump can't be used to
//               reset anyone's password, because the raw token can't be recovered from its
//               hash. ExpiresAt is checked on top of Used, so a token is only valid for a
//               short window (see UserService.ForgotPasswordAsync) even if never consumed.
// Entities connected: User.cs (the account this token would reset)
// Tables related: TBL_PASSWORD_RESET_TOKENS (mapping in Data/Configurations/
//                 PasswordResetTokenConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_PASSWORD_RESET_TOKENS.
/// </summary>
public class PasswordResetToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
