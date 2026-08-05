// =====================================================================================
// FILE SUMMARY
// What it does: Contract for a proposal's full lifecycle: create, list, accept, confirm
//               arrival, upload evidence, confirm closing. ServiceProfessionalsController.cs
//               depends on this interface.
// Entities connected: ServiceProfessional.cs, Service.cs
// Tables related: TBL_SERVICE_PROFESSIONALS, TBL_SERVICES (indirectly)
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IServiceProfessionalService
{
    Task<ServiceProfessionalResponse> CreateProposalAsync(int professionalUserId, int serviceId, CreateServiceProfessionalRequest request);
    Task<IReadOnlyList<ServiceProfessionalResponse>> GetProposalsAsync(int clientUserId, int serviceId);
    Task<ServiceProfessionalResponse> AcceptProposalAsync(int clientUserId, int serviceId, int proposalId);
    Task ConfirmArrivalAsync(int professionalUserId, int serviceId);
    Task UploadCompletionPhotoAsync(int professionalUserId, int serviceId, string photoUrl);
    Task<CompletionStatusResponse> ConfirmCompletionAsync(int callerUserId, int serviceId);
}
