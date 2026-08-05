// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe lo que la API devuelve al consultar un usuario. A propósito NO
//           incluye PasswordHash — nunca se debe exponer, ni siquiera el hash, en una
//           respuesta HTTP. UserService.cs arma este objeto a partir de un User.cs real.
// Entidades relacionadas: User.cs (UserService.cs mapea de una a la otra)
// Tablas relacionadas: Ninguna directamente — es la "forma pública" de una fila de
//                       TBL_USERS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public DateTime RegistrationDate { get; set; }
}
