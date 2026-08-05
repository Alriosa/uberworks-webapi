// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for ServiceProfessional (the proposals) — find by id,
//               list all proposals on a Service, find the Accepted proposal for a Service
//               (to know who "the accepted professional" is), check if a professional
//               already proposed before, create/update one or several.
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
    Task AddAsync(ServiceProfessional serviceProfessional);
    Task UpdateAsync(ServiceProfessional serviceProfessional);
    Task UpdateRangeAsync(IEnumerable<ServiceProfessional> serviceProfessionals);
}
