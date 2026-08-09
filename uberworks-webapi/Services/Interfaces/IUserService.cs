// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the user business logic. UsersController.cs depends on this
//               interface, not on UserService.cs directly — so Program.cs decides in one
//               single place (Extensions/ServiceCollectionExtensions.cs) which real
//               implementation gets wired up. GetByIdAsync/UpdateAsync take the caller's
//               identity (id + role) because only the profile owner or an Admin/MasterAdmin
//               is allowed to view or edit it — see ownership check in UserService.cs.
//               CreateByAdminAsync is the "dedicated endpoint" that RegisterAsync's comment
//               refers to: it lets an already-authenticated Admin/MasterAdmin create Admin,
//               Client, or Professional accounts (never another MasterAdmin).
//               ExternalLoginAsync backs Google AND Facebook sign-in: the WebApp already
//               verified the email with the provider, so this either logs an existing user
//               in (linking FacebookId the first time) or auto-creates a new Role=Client
//               account — never MasterAdmin/Admin. New accounts have no real password
//               (IsPasswordSet=false); SetPasswordAsync is how an already-authenticated
//               caller sets one for the first time.
//               ForgotPasswordAsync/ResetPasswordAsync back the "forgot password" email
//               flow (PasswordResetToken.cs + IEmailSender.cs). ForgotPasswordAsync never
//               reveals whether an email exists; ResetPasswordAsync treats an unknown,
//               expired, or already-used token identically (one generic error).
// Entities connected: User.cs
// Tables related: TBL_USERS (indirectly, via UserService.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserResponse> GetByIdAsync(int id, int callerUserId, UserRole callerRole);
    Task<IReadOnlyList<AdminUserListItemResponse>> GetAllForAdminAsync();
    Task<UserResponse> UpdateAsync(int id, int callerUserId, string callerUsername, UserRole callerRole, UpdateUserRequest request);

    /// <summary>
    /// Admin dashboard's "editarlo absolutamente todo" — edits every field of ANY user
    /// (Username/Email/Role/Status included), unlike UpdateAsync above (self-service,
    /// FirstName/LastName/Phone only). Admin/MasterAdmin only, enforced by
    /// [Authorize(Roles = "MasterAdmin,Admin")] on the controller and re-checked here
    /// (can't touch the MasterAdmin account, can't assign the MasterAdmin role).
    /// </summary>
    Task<UserResponse> AdminUpdateAsync(int id, int callerUserId, string callerUsername, UserRole callerRole, AdminUpdateUserRequest request);

    /// <summary>
    /// Soft-deletes a user (sets Status=Deleted, see UserStatus.cs for why this isn't a real
    /// SQL DELETE). Admin/MasterAdmin only — enforced both here (can't delete yourself, can't
    /// delete the MasterAdmin) and by [Authorize(Roles = "MasterAdmin,Admin")] on the controller.
    /// </summary>
    Task DeleteAsync(int id, int callerUserId, string callerUsername, UserRole callerRole);

    Task<UserResponse> CreateByAdminAsync(int actorUserId, string actorUsername, UserRole actorRole, AdminCreateUserRequest request);

    /// <summary>
    /// Company/Manager dashboard's "Crear Manager" button. The new Manager is always linked
    /// to the SAME company as the caller — see UserService.CreateManagerAsync for how that
    /// company is resolved depending on whether the caller is a Company or an existing Manager.
    /// </summary>
    Task<UserResponse> CreateManagerAsync(int callerUserId, UserRole callerRole, CompanyCreateManagerRequest request);

    /// <summary>Lets a Manager fetch basic info (name/username) about the company it belongs to.</summary>
    Task<UserResponse> GetMyCompanyAsync(int managerUserId);

    /// <summary>
    /// Sends the "set your password" email for an account a third party just created
    /// (used by ProfessionalService.CreateByCompanyAsync — CreateByAdminAsync/
    /// CreateManagerAsync call the private version of this directly since they're in the
    /// same class).
    /// </summary>
    Task SendPasswordSetupEmailAsync(int userId);
    Task<AuthResponse> ExternalLoginAsync(ExternalLoginRequest request);
    Task SetPasswordAsync(int userId, SetPasswordRequest request);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}
