// =====================================================================================
// FILE SUMMARY
// What it does: Backs the "Editar mi perfil" modal on Views/Dashboard/Client.cshtml. Only
//               FirstName/LastName/Phone are editable — same restriction as the API's
//               PUT /api/users/{id} (Username/Email/Password/Role all need separate flows).
//               Posts to DashboardController.UpdateProfile.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;

namespace uberworks_webapp.Models.ViewModels;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }
}
