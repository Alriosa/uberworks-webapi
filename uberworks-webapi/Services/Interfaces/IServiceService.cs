// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de la lógica de negocio de Service (el "Work Post"). ServicesController.cs
//           depende de esta interface. GetByIdAsync recibe el userId de quien pregunta
//           (puede ser null si es anónimo) porque la respuesta cambia según quién pregunta
//           (dirección exacta visible o no).
// Entidades relacionadas: Service.cs
// Tablas relacionadas: TBL_SERVICES (indirectamente, vía ServiceService.cs)
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
