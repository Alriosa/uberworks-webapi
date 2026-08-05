// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for Professional — find by id, by UserId (to check if
//               a user already has a professional profile), create, update.
//               ProfessionalService.cs and ServiceProfessionalService.cs depend on this
//               interface, not on the concrete implementation.
// Entities connected: Professional.cs
// Tables related: TBL_PROFESSIONALS (indirectly, via ProfessionalRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IProfessionalRepository
{
    Task<Professional?> GetByIdAsync(int id);
    Task<Professional?> GetByUserIdAsync(int userId);
    Task<bool> ExistsByUserIdAsync(int userId);
    Task AddAsync(Professional professional);
    Task UpdateAsync(Professional professional);
}
