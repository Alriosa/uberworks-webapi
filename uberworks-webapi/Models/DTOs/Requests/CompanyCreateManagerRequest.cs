// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/users/company-create-manager. A Company OR
//               one of its own Managers uses this to create a new Manager account, always
//               linked to the SAME company — see UserService.CreateManagerAsync for how the
//               company is resolved (the caller's own id if they're a Company, or their own
//               User.ManagedByCompanyUserId if they're already a Manager). CompanyUserId is
//               intentionally NOT here, same reasoning as CompanyCreateWorkerRequest.cs: it's
//               never taken from what the client sends.
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
    public string Password { get; set; } = string.Empty;
}
