// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de la lógica de negocio de Professional. ProfessionalsController.cs
//           depende de esta interface, no de ProfessionalService.cs directamente.
// Entidades relacionadas: Professional.cs
// Tablas relacionadas: TBL_PROFESSIONALS (indirectamente, vía ProfessionalService.cs)
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IProfessionalService
{
    Task<ProfessionalResponse> CreateAsync(int userId, CreateProfessionalRequest request);
    Task<ProfessionalResponse> GetByIdAsync(int id);
    Task<ProfessionalResponse> GetByUserIdAsync(int userId);
    Task<ProfessionalResponse> UpdateAsync(int id, UpdateProfessionalRequest request);
}
