// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/professionals/company-create. A Company
//               account uses this to create a brand-new worker (User with Role=Professional
//               + its Professional profile) in one call, already linked to the Company via
//               Professional.CompanyUserId. The CompanyUserId itself is intentionally NOT
//               here: it's taken from the authenticated caller's JWT (ICurrentUserService),
//               never from what the client sends, so a Company can only ever create workers
//               under its own account.
// Entities connected: User.cs, Professional.cs (both created by
//                      ProfessionalService.CreateByCompanyAsync)
// Tables related: None directly — reaches TBL_USERS/TBL_PROFESSIONALS through ProfessionalService.cs
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class CompanyCreateWorkerRequest
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
}
