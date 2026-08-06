// =====================================================================================
// FILE SUMMARY
// What it does: Generates and hashes opaque, single-use tokens (used for the password
//               reset link). Unlike PasswordHasher.cs (PBKDF2, deliberately slow, for
//               human-chosen passwords that might be weak/reused), this uses a single
//               SHA256 pass: the token itself is already 256 bits of randomness from
//               RandomNumberGenerator, so there's no weak input to protect against by
//               slowing down the hash — the security here comes entirely from the token
//               being unguessable, not from the hash being slow.
// Entities connected: PasswordResetToken.cs (produces the value for its TokenHash property)
// Tables related: TBL_PASSWORD_RESET_TOKENS.CL_TOKEN_HASH (indirectly)
// =====================================================================================
using System.Security.Cryptography;
using System.Text;

namespace uberworks_webapi.Common.Helpers;

public static class SecureTokenHelper
{
    /// <summary>Generates a new random, URL-safe token to email to the user.</summary>
    public static string GenerateToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

    /// <summary>Hashes a token the same deterministic way every time, so it can be looked up by hash.</summary>
    public static string Hash(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
