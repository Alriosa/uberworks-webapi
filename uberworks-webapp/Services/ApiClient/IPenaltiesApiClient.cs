// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/penalties endpoints.
//               GetMineAsync backs the Professional dashboard's "Advertencias" modal —
//               GET /api/penalties/mine is [Authorize] (any role), returning the caller's
//               own sanctions/warnings.
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IPenaltiesApiClient
{
    Task<List<PenaltyResponse>> GetMineAsync(string accessToken);
}
