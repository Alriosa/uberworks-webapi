// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/professionals endpoints.
//               CreateWorkerAsync/GetMyWorkersAsync back a Company account's dashboard
//               (CompanyController.cs) and take the caller's own JWT because both API
//               endpoints require [Authorize(Roles = nameof(UserRole.Company))].
//               GetByUserIdAsync backs the Professional profile page (ProfileController.cs)
//               and needs no token — GET /api/professionals/by-user/{userId} is public on
//               the API side (see ProfessionalsController.GetByUserId).
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IProfessionalsApiClient
{
    Task<ProfessionalResponse> CreateWorkerAsync(string accessToken, CompanyCreateWorkerRequest request);
    Task<List<ProfessionalResponse>> GetMyWorkersAsync(string accessToken);
    Task<ProfessionalResponse> GetByUserIdAsync(int userId);
}
