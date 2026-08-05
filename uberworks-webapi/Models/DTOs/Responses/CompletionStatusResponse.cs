// =====================================================================================
// FILE SUMMARY
// What it does: This is what POST /api/services/{id}/confirm-completion returns — a small
//               summary of where the job-closing process stands: whether the client already
//               confirmed, whether the professional already confirmed, and whether that
//               made the Service Completed.
// Entities connected: Service.cs (indirectly, via
//                      ServiceProfessionalService.ConfirmCompletionAsync)
// Tables related: None directly — summarizes fields from TBL_SERVICES without being a 1:1
//                 mapping of the full row
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class CompletionStatusResponse
{
    public int ServiceId { get; set; }
    public bool ClientConfirmed { get; set; }
    public bool ProfessionalConfirmed { get; set; }
    public bool IsCompleted { get; set; }
}
