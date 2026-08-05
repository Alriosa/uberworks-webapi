// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es lo que devuelve POST /api/users/login — los datos públicos del usuario
//           (UserResponse), el token JWT que debe guardar el cliente (webapp/mobile), y
//           cuándo expira ese token (para que la app sepa cuándo pedir uno nuevo).
// Entidades relacionadas: User.cs (indirectamente, vía UserResponse)
// Tablas relacionadas: Ninguna (el Token no se guarda en la base de datos, solo se firma
//                       y se devuelve)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class AuthResponse
{
    public UserResponse User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
