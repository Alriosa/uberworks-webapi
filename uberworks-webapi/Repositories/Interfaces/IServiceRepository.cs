// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de acceso a datos para Service — obtener por id, listar los
//           "abiertos" (Status=Pending, para que los profesionales exploren), listar los
//           de un cliente específico, crear, actualizar.
// Entidades relacionadas: Service.cs
// Tablas relacionadas: TBL_SERVICES (indirectamente, vía ServiceRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(int id);
    Task<IReadOnlyList<Service>> GetOpenAsync();
    Task<IReadOnlyList<Service>> GetByClientIdAsync(int clientId);
    Task AddAsync(Service service);
    Task UpdateAsync(Service service);
}
