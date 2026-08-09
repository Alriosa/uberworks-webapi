// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/SendChatMessageRequest.cs —
//               the body POST /api/chats/by-service/{serviceId} expects.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class SendChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
}
