// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the Professional business logic. ProfessionalsController.cs
//               depends on this interface, not on ProfessionalService.cs directly.
//               CreateByCompanyAsync/GetByCompanyUserIdAsync back a Company account's
//               ability to create and list its own workers (see Professional.CompanyUserId).
// Entities connected: Professional.cs
// Tables related: TBL_PROFESSIONALS (indirectly, via ProfessionalService.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IProfessionalService
{
    Task<ProfessionalResponse> CreateAsync(int userId, CreateProfessionalRequest request);
    Task<ProfessionalResponse> GetByIdAsync(int id);
    Task<ProfessionalResponse> GetByUserIdAsync(int userId);
    Task<ProfessionalResponse> UpdateAsync(int id, UpdateProfessionalRequest request);
    Task<ProfessionalResponse> CreateByCompanyAsync(int companyUserId, string companyUsername, CompanyCreateWorkerRequest request);
    Task<List<ProfessionalResponse>> GetByCompanyUserIdAsync(int companyUserId);
}
