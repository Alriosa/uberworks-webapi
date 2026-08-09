// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/services endpoints.
//               GetOpenAsync backs the Professional dashboard's "job offers" list (public,
//               no token needed — matches GET /api/services/open having no [Authorize]).
//               GetMineAsync backs the Client dashboard's "Histórico de trabajos" panel and
//               requires the caller's own JWT, since GET /api/services/mine is
//               [Authorize(Roles = "Client")] on the API side. GetAllForAdminAsync/
//               UpdateForAdminAsync/DeleteForAdminAsync back the Admin dashboard's job CRUD
//               panel (DeleteForAdminAsync is a soft delete — Status=Cancelled).
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IServicesApiClient
{
    Task<List<ServiceResponse>> GetOpenAsync();
    Task<List<ServiceResponse>> GetMineAsync(string accessToken);
    Task<List<AdminServiceListItemResponse>> GetAllForAdminAsync(string accessToken);
    Task<ServiceResponse> UpdateForAdminAsync(string accessToken, int id, UpdateServiceAdminRequest request);
    Task DeleteForAdminAsync(string accessToken, int id);
}
