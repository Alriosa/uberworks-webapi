// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IEventRepository.cs — Dapper runs the actual SQL
// against TBL_EVENTS. No JOINs needed: Event has no enum columns and only one FK
// (CompanyUserId, a plain int) — EventService.cs resolves the company's name itself via
// IUserRepository when it needs it, same N+1-is-fine-for-an-internal-listing reasoning used
// throughout (ServiceService.GetAllForAdminAsync, ReportService.cs).
// Entities connected: Event.cs
// Tables related: TBL_EVENTS
// =====================================================================================
using Dapper;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EventRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private const string SelectColumns = """
        SELECT
            PK_EVENT_ID AS Id,
            CL_COMPANY_USER_ID AS CompanyUserId,
            CL_TITLE AS Title,
            CL_DESCRIPTION AS Description,
            CL_NOT_INCLUDED AS NotIncluded,
            CL_EVENT_DATE AS EventDate,
            CL_LOCATION AS Location,
            CL_PROFESSIONALS_NEEDED_COUNT AS ProfessionalsNeededCount,
            CL_CREATED_AT AS CreatedAt
        FROM TBL_EVENTS
        """;

    public async Task<Event?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Event>(
            $"{SelectColumns} WHERE PK_EVENT_ID = @Id", new { Id = id });
    }

    public async Task<IReadOnlyList<Event>> GetByCompanyUserIdAsync(int companyUserId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var events = await connection.QueryAsync<Event>(
            $"{SelectColumns} WHERE CL_COMPANY_USER_ID = @CompanyUserId ORDER BY CL_EVENT_DATE DESC",
            new { CompanyUserId = companyUserId });

        return events.ToList();
    }

    public async Task AddAsync(Event @event)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_EVENTS (CL_COMPANY_USER_ID, CL_TITLE, CL_DESCRIPTION, CL_NOT_INCLUDED, CL_EVENT_DATE, CL_LOCATION, CL_PROFESSIONALS_NEEDED_COUNT)
            OUTPUT INSERTED.PK_EVENT_ID AS Id, INSERTED.CL_CREATED_AT AS CreatedAt
            VALUES (@CompanyUserId, @Title, @Description, @NotIncluded, @EventDate, @Location, @ProfessionalsNeededCount)
            """;

        var generated = await connection.QuerySingleAsync(sql, new
        {
            @event.CompanyUserId,
            @event.Title,
            @event.Description,
            @event.NotIncluded,
            @event.EventDate,
            @event.Location,
            @event.ProfessionalsNeededCount
        });

        @event.Id = (int)generated.Id;
        @event.CreatedAt = (DateTime)generated.CreatedAt;
    }
}
