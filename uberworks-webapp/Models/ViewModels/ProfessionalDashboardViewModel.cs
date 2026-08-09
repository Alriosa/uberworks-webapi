// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Professional.cshtml. AverageRating (from
//               GET /api/professionals/by-user/{userId}) and OpenJobOffers (from
//               GET /api/services/open) are REAL data. OpenJobOffers shows every open Work
//               Post site-wide — there's no Professional↔WorkType relationship in the schema
//               yet to filter it down to only the categories this professional actually does
//               (Professional.cs has no WorkTypes collection, only ServiceProfessional.cs,
//               which links to a specific Service, not a category in general) — see the view
//               for the note on this being a known gap, not an oversight. Penalties (from
//               GET /api/penalties/mine) and CompletedJobs (from
//               GET /api/services/mine-as-professional) are ALSO real now — Penalties backs
//               the "Advertencias" modal, CompletedJobs backs "Trabajos Realizados" (both per
//               explicit request: "también debe generar su modal", and "no solo me liste el
//               historial, sino que yo pueda acceder internamente a ver qué fue lo que se
//               hizo"). Everything else the view shows (monthly earnings, undisbursed
//               balance, active reports, client counts, per-category review breakdown) is
//               still decorative/mock: Payment.cs exists as an entity but has no
//               Repository/Service/Controller built yet, and Review.cs has no per-category
//               aggregation endpoint either.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_PROFESSIONALS/TBL_SERVICES/TBL_PENALTIES only
//                 indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class ProfessionalDashboardViewModel : DashboardViewModelBase
{
    public decimal AverageRating { get; set; }
    public List<ServiceResponse> OpenJobOffers { get; set; } = new();
    public List<PenaltyResponse> Penalties { get; set; } = new();
    public List<ServiceResponse> CompletedJobs { get; set; } = new();
}
