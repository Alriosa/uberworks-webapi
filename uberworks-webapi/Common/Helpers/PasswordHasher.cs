// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Convierte una contraseña en texto plano (ej. "MiClave123") en un texto
//           irreversible ("hash") que sí se puede guardar en la base de datos de forma
//           segura. Usa el algoritmo PBKDF2 (estándar de la industria), sin depender de
//           ninguna librería externa — todo con clases nativas de .NET
//           (System.Security.Cryptography). Se usa: (1) al registrar un usuario, para
//           convertir su password antes de guardarlo, y (2) al hacer login, para comparar
//           el password que mandan contra el hash guardado (sin nunca "des-hashear" nada).
// Entidades relacionadas: User.cs (la propiedad User.PasswordHash se genera con esta clase)
// Tablas relacionadas: TBL_USERS.CL_PASSWORD (ahí se guarda el resultado de Hash(), nunca
//                       el password real)
// =====================================================================================
using System.Security.Cryptography;

namespace uberworks_webapi.Common.Helpers;

/// <summary>
/// Hashing de contraseñas con PBKDF2 (sin dependencias externas).
/// Formato almacenado: "{iteraciones}.{saltBase64}.{hashBase64}".
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
