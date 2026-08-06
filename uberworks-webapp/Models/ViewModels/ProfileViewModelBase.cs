// =====================================================================================
// FILE SUMMARY
// What it does: Common fields every role's profile view shows — the plain User data that
//               comes back from GET /api/users/{id} (UserResponse). Each role has its own
//               subclass (ClientProfileViewModel.cs, ProfessionalProfileViewModel.cs, etc.)
//               adding whatever extra data is specific to that role, and its own .cshtml
//               under Views/Profile — see ProfileController.cs for why each role gets a
//               completely separate view instead of one shared view with conditionals.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class ProfileViewModelBase
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
}
