// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Professional.cshtml. Adds AverageRating, pulled from
//               GET /api/professionals/by-user/{userId}, on top of DashboardViewModelBase.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class ProfessionalDashboardViewModel : DashboardViewModelBase
{
    public decimal AverageRating { get; set; }
}
