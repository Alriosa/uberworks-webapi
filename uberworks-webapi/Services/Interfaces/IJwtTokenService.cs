// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de "generar un token JWT a partir de un usuario". UserService.cs lo
//           usa dentro de LoginAsync() para emitir el token que el cliente (webapp/mobile)
//           va a usar en cada petición futura. Ver explicación paso a paso de JWT al final
//           de la respuesta del chat.
// Entidades relacionadas: User.cs (input del método)
// Tablas relacionadas: Ninguna directamente (no consulta la base de datos)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Services.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
