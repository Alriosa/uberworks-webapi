// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato del ciclo de vida completo de una propuesta: crear, listar, aceptar,
//           confirmar llegada, subir evidencia, confirmar cierre. ServiceProfessionalsController.cs
//           depende de esta interface.
// Entidades relacionadas: ServiceProfessional.cs, Service.cs
// Tablas relacionadas: TBL_SERVICE_PROFESSIONALS, TBL_SERVICES (indirectamente)
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
