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
    Task<UserResponse> UpdateAsync(int id, int callerUserId, string callerUsername, UserRole callerRole, UpdateUserRequest request);
    Task<UserResponse> CreateByAdminAsync(int actorUserId, string actorUsername, UserRole actorRole, AdminCreateUserRequest request);
}
