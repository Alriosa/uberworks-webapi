// =====================================================================================
// FILE SUMMARY
// What it does: Common fields every role's dashboard view needs — just enough to greet
//               the user. DisplayName is actually the Username (from ClaimTypes.Name — see
//               AppClaimsFactory.cs), not FirstName; a real name would need an extra API
//               call the dashboard doesn't otherwise need. Each role has its own subclass
//               (ClientDashboardViewModel.cs, ProfessionalDashboardViewModel.cs, etc.)
//               adding whatever summary data is specific to that role, and its own .cshtml
//               under Views/Dashboard — see DashboardController.cs for why each role gets a
//               completely separate view instead of one generic dashboard.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class DashboardViewModelBase
{
    public string DisplayName { get; set; } = string.Empty;
}
