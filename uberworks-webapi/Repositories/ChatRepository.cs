// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IChatRepository.cs — Dapper runs the actual SQL
//               against TBL_CHATS. No JOINs: ChatService.cs resolves the human-readable
//               sender name itself via IUserRepository/IProfessionalRepository, same
//               N+1-is-fine reasoning as ReportRepository.cs. CL_SENDER_ROLE goes through
//               ChatSenderRoleMapper explicitly — see UserRoleMapper.cs's FILE SUMMARY for
//               why Dapper can't be trusted to convert that column by itself.
// Entities connected: Chat.cs
// Tables related: TBL_CHATS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ChatRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // Raw row shape — CL_SENDER_ROLE stays a string here (see FILE SUMMARY); ToChat()
    // converts it into the real Chat.
    private class ChatRow
    {
        public int Id { get; set; }
        public int ProfessionalId { get; set; }
        public int ClientId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime MessageDate { get; set; }
        public string SenderRole { get; set; } = string.Empty;

        public Chat ToChat() => new()
        {
            Id = Id,
            ProfessionalId = ProfessionalId,
            ClientId = ClientId,
            Message = Message,
            MessageDate = MessageDate,
            SenderRole = ChatSenderRoleMapper.FromDb(SenderRole)
        };
    }

    private const string SelectColumns = """
        SELECT
            PK_CHAT_ID AS Id,
            PK_PROFESSIONAL_ID AS ProfessionalId,
            CL_CLIENT_ID AS ClientId,
            CL_MESSAGE AS Message,
            CL_MESSAGE_DATE AS MessageDate,
            CL_SENDER_ROLE AS SenderRole
        FROM TBL_CHATS
        """;

    public async Task<IReadOnlyList<Chat>> GetConversationAsync(int professionalId, int clientId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ChatRow>(
            $"{SelectColumns} WHERE PK_PROFESSIONAL_ID = @ProfessionalId AND CL_CLIENT_ID = @ClientId ORDER BY CL_MESSAGE_DATE ASC",
            new { ProfessionalId = professionalId, ClientId = clientId });

        return rows.Select(row => row.ToChat()).ToList();
    }

    public async Task AddAsync(Chat chat)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_CHATS (PK_PROFESSIONAL_ID, CL_CLIENT_ID, CL_MESSAGE, CL_SENDER_ROLE)
            OUTPUT INSERTED.PK_CHAT_ID AS Id, INSERTED.CL_MESSAGE_DATE AS MessageDate
            VALUES (@ProfessionalId, @ClientId, @Message, @SenderRole)
            """;

        var generated = await connection.QuerySingleAsync(sql, new
        {
            chat.ProfessionalId,
            chat.ClientId,
            chat.Message,
            SenderRole = ChatSenderRoleMapper.ToDb(chat.SenderRole)
        });

        chat.Id = (int)generated.Id;
        chat.MessageDate = (DateTime)generated.MessageDate;
    }
}
