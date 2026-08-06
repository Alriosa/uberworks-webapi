// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Profile/Professional.cshtml. Adds the worker-specific fields
//               that come from GET /api/professionals/by-user/{userId} (ProfessionalResponse)
//               on top of the plain User data in ProfileViewModelBase. CompanyUserId lets
//               the view show "managed by a company" for workers created via
//               ProfessionalService.CreateByCompanyAsync.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class ProfessionalProfileViewModel : ProfileViewModelBase
{
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
    public decimal AverageRating { get; set; }
    public int? CompanyUserId { get; set; }
}
