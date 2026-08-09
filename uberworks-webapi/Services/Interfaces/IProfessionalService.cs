// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the Professional business logic. ProfessionalsController.cs
//               depends on this interface, not on ProfessionalService.cs directly.
//               CreateByCompanyAsync/GetByCompanyUserIdAsync back a Company account's
//               ability to create and list its own workers (see Professional.CompanyUserId).
//               GetAcceptedWorkTypesAsync backs the "trabajos que puede realizar" section on
//               the Professional profile page — up to 3 real WorkType categories this
//               professional has actually had a proposal accepted/completed on (see
//               IServiceProfessionalRepository.GetAcceptedWorkTypeNamesAsync).
//               UpdateAsync takes the caller's identity (id + role) because only the profile
//               owner or an Admin/MasterAdmin is allowed to edit it — same
//               EnsureSelfOrAdmin-style ownership check as UserService.UpdateAsync.
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
    Task<IReadOnlyList<string>> GetAcceptedWorkTypesAsync(int userId);
    Task<ProfessionalResponse> UpdateAsync(int id, int callerUserId, UserRole callerRole, UpdateProfessionalRequest request);
    Task<ProfessionalResponse> UpdatePhotoAsync(int id, int callerUserId, UserRole callerRole, string photoUrl);
    Task<ProfessionalResponse> CreateByCompanyAsync(int companyUserId, string companyUsername, CompanyCreateWorkerRequest request);
    Task<List<ProfessionalResponse>> GetByCompanyUserIdAsync(int companyUserId);

    /// <summary>
    /// Same listing as GetByCompanyUserIdAsync, but resolves WHICH company from the caller's
    /// own identity — works whether the caller is the Company itself or one of its Managers.
    /// Backs the Manager dashboard, which shows the exact same worker list as its Company.
    /// </summary>
    Task<List<ProfessionalResponse>> GetMyCompanyWorkersAsync(int callerUserId, UserRole callerRole);

    /// <summary>
    /// Links an EXISTING Professional-role account to the caller's company (Company itself,
    /// or one of its Managers — see UserService.ResolveCompanyUserIdAsync for the identical
    /// resolution logic). Searched by email/username/phone, one field. Fails if that
    /// Professional is already linked to a DIFFERENT company.
    /// </summary>
    Task<ProfessionalResponse> LinkExistingWorkerAsync(int callerUserId, UserRole callerRole, string contact);

    /// <summary>Removes a worker from the caller's company (CompanyUserId set back to null).</summary>
    Task UnlinkWorkerAsync(int callerUserId, UserRole callerRole, int professionalId);
}
