// =====================================================================================
// FILE SUMMARY
// What it does: What GET /api/users returns to a MasterAdmin/Admin/Manager caller — one row
//               per user, with every non-sensitive attribute (still never PasswordHash).
//               Backs the "Ver Todos los Usuarios" directory on the WebApp's MasterAdmin
//               dashboard (Views/Dashboard/MasterAdmin.cshtml), which the user asked to show
//               ALL attributes per user instead of a curated subset, to decide later which
//               ones are actually worth keeping visible.
// Entities connected: User.cs (UserService.GetAllForAdminAsync maps a list of these)
// Tables related: None directly — it's the "public admin shape" of a TBL_USERS row
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class AdminUserListItemResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string? FacebookId { get; set; }
    public bool IsPasswordSet { get; set; }
}
