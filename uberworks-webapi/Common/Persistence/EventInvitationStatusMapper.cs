// =====================================================================================
// FILE SUMMARY
// What it does: Converts EventInvitationStatus to/from the text value stored in
//               TBL_EVENT_INVITATIONS.CL_STATUS. No special cases — Pending/Accepted/
//               Declined uppercase cleanly both ways. Called explicitly by
//               EventInvitationRepository.cs — see UserRoleMapper.cs's FILE SUMMARY for why
//               this is a plain static method call instead of a registered Dapper TypeHandler.
// Entities connected: EventInvitation.cs
// Tables related: TBL_EVENT_INVITATIONS.CL_STATUS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class EventInvitationStatusMapper
{
    public static string ToDb(EventInvitationStatus value) => value.ToString().ToUpperInvariant();
    public static EventInvitationStatus FromDb(string value) => Enum.Parse<EventInvitationStatus>(value, ignoreCase: true);
}
