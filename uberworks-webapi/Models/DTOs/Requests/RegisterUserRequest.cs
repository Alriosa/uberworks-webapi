// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe exactamente qué campos debe mandar el cliente (webapp/mobile) en el
//           body de POST /api/users/register. Es una clase "de transporte" (DTO = Data
//           Transfer Object) — nunca se guarda directo en la base de datos; UserService.cs
//           la lee y arma un User.cs a partir de estos datos (más el hash del password).
// Entidades relacionadas: User.cs (UserService.RegisterAsync la convierte en un User)
// Tablas relacionadas: Ninguna directamente — solo llega a TBL_USERS después de pasar por
//                       UserService.cs
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Requests;

public class RegisterUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
