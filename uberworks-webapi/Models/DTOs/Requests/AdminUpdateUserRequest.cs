// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of PUT /api/users/{id}/admin-update — the Admin
// dashboard's "editarlo absolutamente todo" requirement, distinct from the existing
// PUT /api/users/{id} (UpdateUserRequest.cs), which only lets someone edit their own
// FirstName/LastName/Phone. This one lets an Admin/MasterAdmin change every editable
// attribute of ANY user, including Username/Email/Role/Status — see
// UserService.AdminUpdateAsync for the guards (can't touch the MasterAdmin account, can't
// assign the MasterAdmin role, still checks Username/Email uniqueness).
// Entities connected: User.cs (indirectly, via UserService.AdminUpdateAsync)
// Tables related: None directly (TBL_USERS is updated from UserService.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Requests;

public class AdminUpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
}
