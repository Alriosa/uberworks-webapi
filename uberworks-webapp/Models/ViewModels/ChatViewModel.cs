// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Chat.cshtml — the real conversation view between a
//               Client and the Professional accepted on their Service, per explicit request
//               ("iniciar chat con la persona cliente, para hablar sobre el trabajo").
//               ViewerRole (the logged-in caller's own role) is what lets the view align
//               "yours" (right, brand color) vs "theirs" (left, gray) — a message's
//               ChatMessageResponse.SenderRole is compared against it.
// Entities connected: None — this project has no database entities
// Tables related: None — Messages reaches TBL_CHATS only indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class ChatViewModel
{
    public int ServiceId { get; set; }
    public List<ChatMessageResponse> Messages { get; set; } = new();
    public UserRole ViewerRole { get; set; }
}
