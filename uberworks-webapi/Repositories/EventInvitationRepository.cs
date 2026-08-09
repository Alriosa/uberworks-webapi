// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IEventInvitationRepository.cs. CL_STATUS goes
// through EventInvitationStatusMapper explicitly — see UserRoleMapper.cs's FILE SUMMARY for
// why Dapper can't be trusted to convert that column by itself. AddRangeAsync runs one
// INSERT per professional inside a single connection (not a single multi-row statement) —
// simplest correct thing for the handful of workers a company realistically has; not
// optimized for hundreds.
// Entities connected: EventInvitation.cs
// Tables related: TBL_EVENT_INVITATIONS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class EventInvitationRepository : IEventInvitationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EventInvitationRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private class InvitationRow
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ProfessionalUserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        public EventInvitation ToInvitation() => new()
        {
            Id = Id,
            EventId = EventId,
            ProfessionalUserId = ProfessionalUserId,
            Status = EventInvitationStatusMapper.FromDb(Status),
            CreatedAt = CreatedAt,
            RespondedAt = RespondedAt
        };
    }

    private const string SelectColumns = """
        SELECT
            PK_EVENT_INVITATION_ID AS Id,
            PK_EVENT_ID AS EventId,
            CL_PROFESSIONAL_USER_ID AS ProfessionalUserId,
            CL_STATUS AS Status,
            CL_CREATED_AT AS CreatedAt,
            CL_RESPONDED_AT AS RespondedAt
        FROM TBL_EVENT_INVITATIONS
        """;

    public async Task<EventInvitation?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<InvitationRow>(
            $"{SelectColumns} WHERE PK_EVENT_INVITATION_ID = @Id", new { Id = id });

        return row?.ToInvitation();
    }

    public async Task<IReadOnlyList<EventInvitation>> GetByEventIdAsync(int eventId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<InvitationRow>(
            $"{SelectColumns} WHERE PK_EVENT_ID = @EventId", new { EventId = eventId });

        return rows.Select(r => r.ToInvitation()).ToList();
    }

    public async Task<IReadOnlyList<EventInvitation>> GetByProfessionalUserIdAsync(int professionalUserId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<InvitationRow>(
            $"{SelectColumns} WHERE CL_PROFESSIONAL_USER_ID = @ProfessionalUserId ORDER BY CL_CREATED_AT DESC",
            new { ProfessionalUserId = professionalUserId });

        return rows.Select(r => r.ToInvitation()).ToList();
    }

    public async Task AddRangeAsync(IEnumerable<EventInvitation> invitations)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_EVENT_INVITATIONS (PK_EVENT_ID, CL_PROFESSIONAL_USER_ID, CL_STATUS)
            VALUES (@EventId, @ProfessionalUserId, @Status)
            """;

        foreach (var invitation in invitations)
        {
            await connection.ExecuteAsync(sql, new
            {
                invitation.EventId,
                invitation.ProfessionalUserId,
                Status = EventInvitationStatusMapper.ToDb(invitation.Status)
            });
        }
    }

    public async Task UpdateAsync(EventInvitation invitation)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE TBL_EVENT_INVITATIONS
            SET CL_STATUS = @Status,
                CL_RESPONDED_AT = @RespondedAt
            WHERE PK_EVENT_INVITATION_ID = @Id
            """;

        await connection.ExecuteAsync(sql, new
        {
            invitation.Id,
            Status = EventInvitationStatusMapper.ToDb(invitation.Status),
            invitation.RespondedAt
        });
    }
}
