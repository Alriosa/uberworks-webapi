// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/professionals endpoints.
//               CreateWorkerAsync/GetMyWorkersAsync back a Company account's dashboard
//               (CompanyController.cs) and take the caller's own JWT because both API
//               endpoints require [Authorize(Roles = nameof(UserRole.Company))].
//               GetByUserIdAsync backs the Professional profile page (ProfileController.cs)
//               and needs no token — GET /api/professionals/by-user/{userId} is public on
//               the API side (see ProfessionalsController.GetByUserId).
//               GetMyAcceptedWorkTypesAsync backs that same page's "trabajos que puede
//               realizar" section (real data — see ProfessionalsController.GetMyAcceptedWorkTypes
//               on the API side). UpdateAsync backs that page's "Editar"/"Guardar" flow for
//               the Professional-specific fields (Description/Experience/Availability/Location).
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using Microsoft.AspNetCore.Http;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IProfessionalsApiClient
{
    Task<ProfessionalResponse> CreateWorkerAsync(string accessToken, CompanyCreateWorkerRequest request);
    Task<List<ProfessionalResponse>> GetMyWorkersAsync(string accessToken);
    Task<ProfessionalResponse> GetByUserIdAsync(int userId);
    Task<List<string>> GetMyAcceptedWorkTypesAsync(string accessToken);
    Task<ProfessionalResponse> UpdateAsync(string accessToken, int id, UpdateProfessionalRequest request);
    Task<ProfessionalResponse> UploadPhotoAsync(string accessToken, int id, IFormFile photo);

    /// <summary>Also usable by a Manager — GetMyWorkersAsync/LinkExistingAsync/UnlinkAsync all resolve to the SAME company.</summary>
    Task<ProfessionalResponse> LinkExistingAsync(string accessToken, string contact);
    Task UnlinkAsync(string accessToken, int professionalId);
}
