// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Professional.cshtml. AverageRating (from
//               GET /api/professionals/by-user/{userId}) and OpenJobOffers (from
//               GET /api/services/open) are REAL data. OpenJobOffers shows every open Work
//               Post site-wide — there's no Professional↔WorkType relationship in the schema
//               yet to filter it down to only the categories this professional actually does
//               (Professional.cs has no WorkTypes collection, only ServiceProfessional.cs,
//               which links to a specific Service, not a category in general) — see the view
//               for the note on this being a known gap, not an oversight. Everything else the
//               view shows (monthly earnings, undisbursed balance, active reports, warnings,
//               client/job counts, per-category review breakdown) is still decorative/mock:
//               Payment.cs and Penalty.cs both exist as entities but have no
//               Repository/Service/Controller built yet, and Review.cs has no per-category
//               aggregation endpoint either.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_PROFESSIONALS/TBL_SERVICES only indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class ProfessionalDashboardViewModel : DashboardViewModelBase
{
    public decimal AverageRating { get; set; }
    public List<ServiceResponse> OpenJobOffers { get; set; } = new();
}
