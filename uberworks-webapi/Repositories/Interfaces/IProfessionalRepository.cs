// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de acceso a datos para Professional — buscar por id, por UserId
//           (para saber si un usuario ya tiene perfil profesional), crear, actualizar.
//           ProfessionalService.cs y ServiceProfessionalService.cs dependen de esta
//           interface, no de la implementación concreta.
// Entidades relacionadas: Professional.cs
// Tablas relacionadas: TBL_PROFESSIONALS (indirectamente, vía ProfessionalRepository.cs)
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
