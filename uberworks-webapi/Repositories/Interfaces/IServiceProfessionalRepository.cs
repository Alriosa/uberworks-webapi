// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for ServiceProfessional (the proposals) — find by id,
//               list all proposals on a Service, find the Accepted proposal for a Service
//               (to know who "the accepted professional" is), check if a professional
//               already proposed before, create/update one or several.
//               GetAcceptedWorkTypeNamesAsync backs the "trabajos que puede realizar" section
//               on the Professional profile page (Views/Profile/ProfileProfessional.cshtml) —
//               the real, distinct list of WorkType categories this professional has actually
//               had a proposal accepted/completed on, capped to `limit`.
//               GetAcceptedServiceIdsAsync backs the Professional dashboard's "Trabajos
//               Realizados" panel — every Service id this professional has been accepted,
//               in progress, or completed on, most recent first (ServiceService.cs turns
//               these ids into full ServiceResponse detail).
// Entities connected: ServiceProfessional.cs
// Tables related: TBL_SERVICE_PROFESSIONALS (indirectly, via ServiceProfessionalRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IServiceProfessionalRepository
{
    Task<ServiceProfessional?> GetByIdAsync(int id);
    Task<IReadOnlyList<ServiceProfessional>> GetByServiceIdAsync(int serviceId);
    Task<ServiceProfessional?> GetAcceptedForServiceAsync(int serviceId);
    Task<bool> ExistsProposalAsync(int serviceId, int professionalId);
    Task<IReadOnlyList<string>> GetAcceptedWorkTypeNamesAsync(int professionalId, int limit);
    Task<IReadOnlyList<int>> GetAcceptedServiceIdsAsync(int professionalId);
    Task AddAsync(ServiceProfessional serviceProfessional);
    Task UpdateAsync(ServiceProfessional serviceProfessional);
    Task UpdateRangeAsync(IEnumerable<ServiceProfessional> serviceProfessionals);
}
