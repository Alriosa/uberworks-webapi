// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/users endpoints over HTTP.
//               AccountController.cs/AdminController.cs depend on this interface, not on
//               UsersApiClient.cs directly, matching the same Controller→Service pattern
//               used in the API itself. AdminCreateUserAsync takes the caller's own JWT
//               (pulled from their auth cookie's "access_token" claim) because
//               POST /api/users/admin-create requires [Authorize(Roles = "MasterAdmin,Admin")]
//               on the API side — without a valid Bearer token from an admin, the API
//               rejects the call with 401/403 before it even reaches the business logic.
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
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task<UserResponse> GetByIdAsync(string accessToken, int id);
}
