// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for Service — get by id, list the "open" ones
//               (Status=Pending, for professionals to browse), list a specific client's,
//               list ALL of them (Admin dashboard), create, update.
// Entities connected: Service.cs
// Tables related: TBL_SERVICES (indirectly, via ServiceRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(int id);
    Task<IReadOnlyList<Service>> GetOpenAsync();
    Task<IReadOnlyList<Service>> GetByClientIdAsync(int clientId);

    /// <summary>Every Service regardless of status — backs the Admin dashboard's job CRUD panel.</summary>
    Task<IReadOnlyList<Service>> GetAllAsync();

    Task AddAsync(Service service);
    Task UpdateAsync(Service service);
}
