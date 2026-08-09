// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Profile/ProfileProfessional.cshtml. Adds the worker-specific
//               fields that come from GET /api/professionals/by-user/{userId}
//               (ProfessionalResponse) on top of the plain User data in
//               ProfileViewModelBase. CompanyUserId lets the view show "managed by a
//               company" for workers created via ProfessionalService.CreateByCompanyAsync.
//               AcceptedWorkTypes (from GET /api/professionals/my-accepted-worktypes) is REAL
//               data — up to 3 WorkType categories this professional has actually been hired
//               for, backing the "Trabajos que Puede Realizar" section. EditProfile pre-fills
//               the "Editar Mi Perfil" modal.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class ProfessionalProfileViewModel : ProfileViewModelBase
{
    public int ProfessionalId { get; set; }
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
    public decimal AverageRating { get; set; }
    public int? CompanyUserId { get; set; }
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// The API's own base URL (e.g. "https://localhost:7202/") — PhotoUrl is only a relative
    /// path on the API's host ("/uploads/professional-photos/..."), so the view has to
    /// prepend this to actually load the image, since the WebApp and API are two different
    /// hosts/ports. See ProfileController.LandingPage.
    /// </summary>
    public string ApiBaseUrl { get; set; } = string.Empty;
    public List<string> AcceptedWorkTypes { get; set; } = new();
    public EditProfessionalProfileViewModel EditProfile { get; set; } = new();
}
