// =====================================================================================
// FILE SUMMARY
// What it does: Defines whether a professional has responded to a Company's Event
//               invitation yet, and how. Every invitation starts Pending — see
//               EventService.CreateAsync, which generates one per professional currently
//               linked to the company.
// Entities connected: EventInvitation.cs (the EventInvitation.Status property is of this type)
// Tables related: TBL_EVENT_INVITATIONS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

public enum EventInvitationStatus
{
    Pending,
    Accepted,
    Declined
}
