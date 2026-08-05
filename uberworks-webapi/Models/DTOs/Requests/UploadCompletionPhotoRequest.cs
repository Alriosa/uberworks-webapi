// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/services/{serviceId}/completion-photo. For
//               now it only receives a URL of a photo already uploaded to some external
//               storage — the actual file-upload endpoint (multipart/form-data) isn't built
//               yet, it's a pending piece.
// Entities connected: Service.cs (indirectly, via
//                      ServiceProfessionalService.UploadCompletionPhotoAsync)
// Tables related: None directly (TBL_SERVICES.CL_COMPLETION_PHOTO_URL is updated from
//                 ServiceProfessionalService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

// For now this receives an already-uploaded URL (to some external storage).
// The actual file-upload endpoint (multipart/form-data) is designed separately.
public class UploadCompletionPhotoRequest
{
    public string PhotoUrl { get; set; } = string.Empty;
}
