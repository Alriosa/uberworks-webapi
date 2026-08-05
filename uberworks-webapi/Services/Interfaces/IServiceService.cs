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
    Task<ServiceResponse> GetByIdAsync(int serviceId, int? callerUserId);
}
