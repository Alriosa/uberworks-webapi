// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/RespondToInvitationRequest.cs
//               — the body POST /api/events/invitations/{id}/respond expects. Backs the
//               Accept/Decline buttons on Views/Dashboard/EventInvitations.cshtml.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class RespondToInvitationRequest
{
    public bool Accept { get; set; }
}
