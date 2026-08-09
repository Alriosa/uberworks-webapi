// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/users endpoints over HTTP.
//               AccountController.cs/AdminController.cs depend on this interface, not on
//               UsersApiClient.cs directly, matching the same Controller→Service pattern
//               used in the API itself. AdminCreateUserAsync/SetPasswordAsync/GetByIdAsync
//               take the caller's own JWT (pulled from their auth cookie's "access_token"
//               claim) because those API endpoints require [Authorize] — without a valid
//               Bearer token, the API rejects the call with 401/403 before it even reaches
//               the business logic.
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IUsersApiClient
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserResponse> RegisterAsync(RegisterUserRequest request);
    Task<UserResponse> AdminCreateUserAsync(string accessToken, AdminCreateUserRequest request);
    Task<AuthResponse> ExternalLoginAsync(ExternalLoginRequest request);
    Task SetPasswordAsync(string accessToken, string newPassword);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task<UserResponse> GetByIdAsync(string accessToken, int id);
    Task<UserResponse> UpdateAsync(string accessToken, int id, UpdateUserRequest request);
    Task<List<AdminUserListItemResponse>> GetAllUsersAsync(string accessToken);

    /// <summary>Soft delete (Status=Deleted) — backs the Admin dashboard's user CRUD panel.</summary>
    Task DeleteAsync(string accessToken, int id);

    /// <summary>Company/Manager dashboard's "Crear Manager" button.</summary>
    Task<UserResponse> CreateManagerAsync(string accessToken, CompanyCreateManagerRequest request);

    /// <summary>Manager dashboard's "nombre de la empresa" display.</summary>
    Task<UserResponse> GetMyCompanyAsync(string accessToken);
}
