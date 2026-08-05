// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the user business logic. UsersController.cs depends on this
//               interface, not on UserService.cs directly — so Program.cs decides in one
//               single place (Extensions/ServiceCollectionExtensions.cs) which real
//               implementation gets wired up.
// Entities connected: User.cs
// Tables related: TBL_USERS (indirectly, via UserService.cs)
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
