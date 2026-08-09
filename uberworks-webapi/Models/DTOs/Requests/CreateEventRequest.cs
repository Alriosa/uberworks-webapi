// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/events — a Company's "Crear Evento" button.
// CompanyUserId is NOT here: it's always the caller's own id (Company-only action, never a
// Manager — see EventsController.Create). Description/NotIncluded together are "toda la
// descripción de que va a ser necesario y que no", per explicit request.
// Entities connected: Event.cs (indirectly, via EventService.CreateAsync)
// Tables related: None directly (TBL_EVENTS is filled in from EventService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class CreateEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? NotIncluded { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int ProfessionalsNeededCount { get; set; }
}
