// =====================================================================================
// FILE SUMMARY
// What it does: Implements IChatService.cs. ResolvePartiesAsync is the one place all the
//               ownership rules live: a Service must exist and already have an ACCEPTED
//               proposal (no accepted professional yet means there's no one to chat with —
//               ConflictException), and the caller must be either that Service's own client
//               or that accepted professional (anyone else — ForbiddenException). Once
//               resolved, both read and send operate on the underlying (ProfessionalId,
//               ClientId) pair that TBL_CHATS is actually keyed by (see Chat.cs's FILE
//               SUMMARY for why there's no ServiceId column). MapToResponse resolves the
//               human-readable sender username via IUserRepository/IProfessionalRepository —
//               N+1 lookups, same reasoning as ReportService.cs, acceptable at this app's scale.
// Entities connected: Chat.cs, Service.cs, ServiceProfessional.cs, Professional.cs, User.cs
// Tables related: TBL_CHATS
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IServiceProfessionalRepository _serviceProfessionalRepository;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IUserRepository _userRepository;

    public ChatService(
        IChatRepository chatRepository,
        IServiceRepository serviceRepository,
        IServiceProfessionalRepository serviceProfessionalRepository,
        IProfessionalRepository professionalRepository,
        IUserRepository userRepository)
    {
        _chatRepository = chatRepository;
        _serviceRepository = serviceRepository;
        _serviceProfessionalRepository = serviceProfessionalRepository;
        _professionalRepository = professionalRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<ChatMessageResponse>> GetConversationByServiceAsync(int serviceId, int callerUserId, UserRole callerRole)
    {
        var (professionalId, clientId, _) = await ResolvePartiesAsync(serviceId, callerUserId, callerRole);
        var messages = await _chatRepository.GetConversationAsync(professionalId, clientId);

        var client = await _userRepository.GetByIdAsync(clientId);
        var professionalUser = await GetProfessionalUserAsync(professionalId);

        return messages.Select(m => MapToResponse(m, client, professionalUser)).ToList();
    }

    public async Task<ChatMessageResponse> SendMessageByServiceAsync(int serviceId, int callerUserId, UserRole callerRole, SendChatMessageRequest request)
    {
        var (professionalId, clientId, senderRole) = await ResolvePartiesAsync(serviceId, callerUserId, callerRole);

        var chat = new Chat
        {
            ProfessionalId = professionalId,
            ClientId = clientId,
            Message = request.Message,
            SenderRole = senderRole
        };

        await _chatRepository.AddAsync(chat);

        var client = await _userRepository.GetByIdAsync(clientId);
        var professionalUser = await GetProfessionalUserAsync(professionalId);

        return MapToResponse(chat, client, professionalUser);
    }

    // The one place every ownership/eligibility rule for chatting about a Service lives.
    private async Task<(int ProfessionalId, int ClientId, ChatSenderRole CallerRole)> ResolvePartiesAsync(int serviceId, int callerUserId, UserRole callerRole)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        var accepted = await _serviceProfessionalRepository.GetAcceptedForServiceAsync(serviceId)
            ?? throw new ConflictException("This service doesn't have an accepted professional yet — there's no one to chat with.");

        if (callerRole == UserRole.Client)
        {
            if (service.ClientId != callerUserId)
            {
                throw new ForbiddenException("You can only chat about your own service requests.");
            }

            return (accepted.ProfessionalId, service.ClientId, ChatSenderRole.Client);
        }

        if (callerRole == UserRole.Professional)
        {
            var professional = await _professionalRepository.GetByUserIdAsync(callerUserId)
                ?? throw new NotFoundException("The authenticated user does not have a professional profile.");

            if (professional.Id != accepted.ProfessionalId)
            {
                throw new ForbiddenException("You are not the accepted professional for this service.");
            }

            return (accepted.ProfessionalId, service.ClientId, ChatSenderRole.Professional);
        }

        throw new ForbiddenException("Only the client and the accepted professional can use this chat.");
    }

    private async Task<User?> GetProfessionalUserAsync(int professionalId)
    {
        var professional = await _professionalRepository.GetByIdAsync(professionalId);
        return professional is null ? null : await _userRepository.GetByIdAsync(professional.UserId);
    }

    private static ChatMessageResponse MapToResponse(Chat chat, User? client, User? professionalUser)
    {
        var senderUsername = chat.SenderRole == ChatSenderRole.Client
            ? client?.Username ?? string.Empty
            : professionalUser?.Username ?? string.Empty;

        return new ChatMessageResponse
        {
            Id = chat.Id,
            ProfessionalId = chat.ProfessionalId,
            ClientId = chat.ClientId,
            Message = chat.Message,
            MessageDate = chat.MessageDate,
            SenderRole = chat.SenderRole,
            SenderUsername = senderUsername
        };
    }
}
