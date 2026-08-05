// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de POST /api/services/{serviceId}/completion-photo. Por ahora
//           solo recibe una URL de una foto ya subida a algún storage externo — el
//           endpoint real de subida de archivos (multipart/form-data) todavía no está
//           construido, es una pieza pendiente.
// Entidades relacionadas: Service.cs (indirectamente, vía
//                          ServiceProfessionalService.UploadCompletionPhotoAsync)
// Tablas relacionadas: Ninguna directamente (TBL_SERVICES.CL_COMPLETION_PHOTO_URL se
//                       actualiza desde ServiceProfessionalService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

// Por ahora se recibe una URL ya subida (a un storage externo).
// El endpoint de subida de archivos en sí (multipart/form-data) se diseña aparte.
public class UploadCompletionPhotoRequest
{
    public string PhotoUrl { get; set; } = string.Empty;
}
