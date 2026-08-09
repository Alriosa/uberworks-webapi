// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/users/company-create-manager. Only a
//               Company can call this (never a Manager — see the [Authorize] on
//               UsersController.CreateManager), and the new Manager is always linked to the
//               caller's own company. CompanyUserId is intentionally NOT here, same
//               reasoning as CompanyCreateWorkerRequest.cs: it's never taken from what the
//               client sends. No Password field either, same reasoning as
//               AdminCreateUserRequest.cs — the new Manager gets a "set your password" email
//               instead (see UserService.SendPasswordSetupLinkAsync).
// Entities connected: User.cs (created by UserService.CreateManagerAsync)
// Tables related: None directly — reaches TBL_USERS through UserService.cs
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class CompanyCreateManagerRequest
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
