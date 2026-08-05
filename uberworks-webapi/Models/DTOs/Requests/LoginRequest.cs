// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de POST /api/users/login (solo email + password). Es un DTO
//           de entrada — UserService.LoginAsync lo usa para buscar al User por email y
//           verificar el password con PasswordHasher.Verify(), sin nunca ver el hash real.
// Entidades relacionadas: User.cs (indirectamente, vía UserService.LoginAsync)
// Tablas relacionadas: Ninguna directamente (TBL_USERS se consulta desde UserService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
