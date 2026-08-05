// =====================================================================================
// FILE SUMMARY
// What it does: Converts a plain-text password (e.g. "MyPassword123") into an irreversible
//               "hash" that can be safely stored in the database. Uses the PBKDF2 algorithm
//               (industry standard), with no external dependencies — everything built with
//               native .NET classes (System.Security.Cryptography). Used: (1) at registration,
//               to convert the password before storing it, and (2) at login, to compare the
//               submitted password against the stored hash, without ever "un-hashing" anything.
// Entities connected: User.cs (the User.PasswordHash property is generated with this class)
// Tables related: TBL_USERS.CL_PASSWORD (Hash() output is stored there, never the real password)
// =====================================================================================
using System.Security.Cryptography;

namespace uberworks_webapi.Common.Helpers;

/// <summary>
/// Password hashing with PBKDF2 (no external dependencies).
/// Stored format: "{iterations}.{saltBase64}.{hashBase64}".
/// </summary>
public static class PasswordHasher
{
    private const int SaltSizeBytes = 16;
    private const int KeySizeBytes = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySizeBytes);

        return string.Join('.', Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    public static bool Verify(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[1]);
        var expectedKey = Convert.FromBase64String(parts[2]);
        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedKey.Length);

        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }
}
