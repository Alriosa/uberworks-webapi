// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es lo que devuelve POST /api/services/{id}/confirm-completion — un resumen
//           chiquito de en qué punto va el cierre del trabajo: si ya confirmó el cliente,
//           si ya confirmó el profesional, y si con eso el Service quedó Completed.
// Entidades relacionadas: Service.cs (indirectamente, vía
//                          ServiceProfessionalService.ConfirmCompletionAsync)
// Tablas relacionadas: Ninguna directamente — resume campos de TBL_SERVICES sin ser un
//                       mapeo 1:1 de toda la fila
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class CompletionStatusResponse
{
    public int ServiceId { get; set; }
    public bool ClientConfirmed { get; set; }
    public bool ProfessionalConfirmed { get; set; }
    public bool IsCompleted { get; set; }
}
