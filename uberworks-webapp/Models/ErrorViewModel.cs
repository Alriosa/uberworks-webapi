// =====================================================================================
// FILE SUMMARY
// What it does: Default MVC scaffold view model, backs Views/Shared/Error.cshtml (the
//               generic error page). Not related to the API's error contract — see
//               Models/ApiContracts/ApiErrorResponse.cs for that.
// Entities connected: None
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
