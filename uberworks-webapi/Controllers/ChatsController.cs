// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the HTTP endpoints for the real Chat system — "iniciar chat con la
//               persona cliente, para hablar sobre el trabajo", per explicit request. Both
//               routes hang off /api/chats/by-service/{serviceId} rather than a raw
//               professionalId/clientId pair — [Authorize] (any logged-in role) is enough at
//               the attribute level because IChatService.ResolvePartiesAsync does the real
//               eligibility check (only the Service's own client or its accepted professional
//               gets in; everyone else gets 403/409).
// Entities connected: Chat.cs (indirectly, via IChatService)
// Tables related: TBL_CHATS (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ICurrentUserService _currentUserService;

    public ChatsController(IChatService chatService, ICurrentUserService currentUserService)
    {
        _chatService = chatService;
        _currentUserService = currentUserService;
    }

    [HttpGet("by-service/{serviceId:int}")]
    public async Task<IActionResult> GetConversation(int serviceId)
    {
        var userId = _currentUserService.UserId!.Value;
        var role = _currentUserService.Role!.Value;
        var result = await _chatService.GetConversationByServiceAsync(serviceId, userId, role);
        return Ok(result);
    }

    [HttpPost("by-service/{serviceId:int}")]
    public async Task<IActionResult> SendMessage(int serviceId, [FromBody] SendChatMessageRequest request)
    {
        var userId = _currentUserService.UserId!.Value;
        var role = _currentUserService.Role!.Value;
        var result = await _chatService.SendMessageByServiceAsync(serviceId, userId, role, request);
        return Ok(result);
    }
}
