// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de acceso a datos para ServiceProfessional (las propuestas) — buscar
//           por id, listar todas las propuestas de un Service, encontrar la propuesta
//           Accepted de un Service (para saber quién es "el profesional aceptado"),
//           verificar si un profesional ya propuso antes, crear/actualizar una o varias.
// Entidades relacionadas: ServiceProfessional.cs
// Tablas relacionadas: TBL_SERVICE_PROFESSIONALS (indirectamente, vía
//                       ServiceProfessionalRepository.cs)
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
