// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the Service ("Work Post") business logic. ServicesController.cs
//               depends on this interface. GetByIdAsync receives the requester's userId (can
//               be null for anonymous callers) because the response changes depending on who
//               is asking (exact address visible or not).
// Entities connected: Service.cs
// Tables related: TBL_SERVICES (indirectly, via ServiceService.cs)
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IServiceService
{
    Task<ServiceResponse> CreateAsync(int clientId, CreateServiceRequest request);
    Task<IReadOnlyList<ServiceResponse>> GetOpenAsync();
    Task<IReadOnlyList<ServiceResponse>> GetMyServicesAsync(int clientId);

    /// <summary>
    /// Backs the Professional dashboard's "Trabajos Realizados" panel — every Service this
    /// professional has been accepted, in progress, or completed on, most recent first, with
    /// the exact location included (same rule as the accepted-professional case in
    /// GetByIdAsync — they did the job, they get to see where it was).
    /// </summary>
    Task<IReadOnlyList<ServiceResponse>> GetMyCompletedJobsAsProfessionalAsync(int professionalUserId);

    Task<ServiceResponse> GetByIdAsync(int serviceId, int? callerUserId);

    /// <summary>Every Service, full detail, regardless of status — Admin dashboard CRUD panel.</summary>
    Task<IReadOnlyList<AdminServiceListItemResponse>> GetAllForAdminAsync();

    Task<ServiceResponse> UpdateForAdminAsync(int serviceId, UpdateServiceAdminRequest request);

    /// <summary>
    /// "Deletes" a job from the Admin dashboard — sets Status=Cancelled rather than a real
    /// SQL DELETE, since ServiceProfessional/Review/Payment rows reference TBL_SERVICES (same
    /// reasoning as UserService.DeleteAsync).
    /// </summary>
    Task DeleteForAdminAsync(int serviceId);
}
