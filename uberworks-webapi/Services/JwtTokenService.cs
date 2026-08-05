// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Construye el token JWT real. Toma los datos del usuario que hizo login
//           (Id, Email, Role) y los mete dentro del token como "claims" (afirmaciones
//           firmadas), usando la SecretKey de appsettings.json/Jwt para firmar. El
//           resultado es un texto largo que el cliente (webapp/mobile) debe mandar en cada
//           petición futura dentro del header "Authorization: Bearer {token}".
//           Explicación paso a paso completa de todo el mecanismo JWT al final de la
//           respuesta del chat donde se agregó este comentario.
// Entidades relacionadas: User.cs (de aquí saca Id, Email, Role para meterlos en el token)
// Tablas relacionadas: Ninguna directamente (no consulta la base de datos, solo firma texto)
// =====================================================================================
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("Falta configurar Jwt:SecretKey en appsettings.");
        var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"] ?? "60");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expiresAtUtc);
    }
}
