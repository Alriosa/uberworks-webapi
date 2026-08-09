// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/ChatMessageResponse.cs — a
//               single message in a conversation. Returned by
//               GET /api/chats/by-service/{serviceId}. Backs Views/Dashboard/Chat.cshtml.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class ChatMessageResponse
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int ClientId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime MessageDate { get; set; }
    public ChatSenderRole SenderRole { get; set; }
    public string SenderUsername { get; set; } = string.Empty;
}
