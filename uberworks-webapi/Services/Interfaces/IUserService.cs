// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de la lógica de negocio de usuarios. UsersController.cs depende de
//           esta interface, no de UserService.cs directamente — así Program.cs decide en
//           un solo lugar (Extensions/ServiceCollectionExtensions.cs) qué implementación
//           real se conecta.
// Entidades relacionadas: User.cs
// Tablas relacionadas: TBL_USERS (indirectamente, vía UserService.cs)
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserResponse> GetByIdAsync(int id);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
}
