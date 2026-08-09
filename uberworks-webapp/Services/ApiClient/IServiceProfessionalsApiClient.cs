// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/services/{serviceId}/...
//               negotiation endpoints (ServiceProfessionalsController.cs on the API side).
//               CreateProposalAsync backs the Professional dashboard's job-offer detail modal
//               (a professional dictating their own price). GetProposalsAsync backs the
//               Client dashboard's "Histórico de Trabajos" detail view — Client-only on the
//               API side, used to find out who (if anyone) was accepted for a given Service.
//               AcceptProposalAsync is for a future "choose a professional" screen — not
//               wired to any view yet, but the API endpoint already exists.
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IServiceProfessionalsApiClient
{
    Task<ServiceProfessionalResponse> CreateProposalAsync(string accessToken, int serviceId, CreateServiceProfessionalRequest request);
    Task<List<ServiceProfessionalResponse>> GetProposalsAsync(string accessToken, int serviceId);
    Task<ServiceProfessionalResponse> AcceptProposalAsync(string accessToken, int serviceId, int proposalId);
}
