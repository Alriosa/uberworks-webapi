// =====================================================================================
// FILE SUMMARY
// What it does: Backs the "Editar" modal on Views/Profile/ProfileProfessional.cshtml. Mixes
//               fields from two different API resources — FirstName/LastName/Phone go to
//               PUT /api/users/{id} (via IUsersApiClient), Description/Experience/
//               Availability/Location go to PUT /api/professionals/{id} (via
//               IProfessionalsApiClient) — ProfileController.UpdateProfessionalProfile calls
//               both in the same POST so the form only has one "Guardar" button.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.ComponentModel.DataAnnotations;

namespace uberworks_webapp.Models.ViewModels;

public class EditProfessionalProfileViewModel
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
}
