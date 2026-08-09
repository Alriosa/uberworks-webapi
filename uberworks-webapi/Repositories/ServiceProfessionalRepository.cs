// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IServiceProfessionalRepository.cs. Uses a
//               3-way Dapper multi-mapping (ServiceProfessional + Professional + User) over
//               a double JOIN — this is what EF Core's
//               ".Include(sp => sp.Professional).ThenInclude(p => p.User)" used to do.
//               Maps into the private ServiceProfessionalRow and UserRow.cs (both keep their
//               enum columns as plain strings) before converting to the real entities via
//               ServiceProfessionalStatusMapper/UserRoleMapper/UserStatusMapper — see
//               UserRoleMapper.cs's FILE SUMMARY for why Dapper can't be trusted to do that
//               conversion by itself. splitOn: "Id,Id" gives Dapper the two points where a
//               new object starts in the row (Professional's columns, then User's) — same
//               idea as ProfessionalRepository.cs, just with one more split.
//               GetAcceptedForServiceAsync() is key for the "exact address" rule (see
//               Services/ServiceService.cs).
// Entities connected: ServiceProfessional.cs, Professional.cs, User.cs (via a real SQL JOIN)
// Tables related: TBL_SERVICE_PROFESSIONALS, TBL_PROFESSIONALS, TBL_USERS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class ServiceProfessionalRepository : IServiceProfessionalRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ServiceProfessionalRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // The raw shape of a ServiceProfessional row — CL_STATUS stays a string here (see FILE
    // SUMMARY); ToServiceProfessional() converts it into the real entity.
    private class ServiceProfessionalRow
    {
        public int Id { get; set; }
        public int ProfessionalId { get; set; }
        public int ServiceId { get; set; }
        public decimal? NegotiatedPrice { get; set; }
        public int EstimatedArrivalMinutes { get; set; }
        public DateTime? ArrivalConfirmedAt { get; set; }
        public string Status { get; set; } = string.Empty;

        public ServiceProfessional ToServiceProfessional() => new()
        {
            Id = Id,
            ProfessionalId = ProfessionalId,
            ServiceId = ServiceId,
            NegotiatedPrice = NegotiatedPrice,
            EstimatedArrivalMinutes = EstimatedArrivalMinutes,
            ArrivalConfirmedAt = ArrivalConfirmedAt,
            Status = ServiceProfessionalStatusMapper.FromDb(Status)
        };
    }

    // sp.* (ServiceProfessionalRow), then p.* (Professional), then u.* (UserRow) — order
    // must match <ServiceProfessionalRow, Professional, UserRow, ServiceProfessional> below.
    private const string SelectWithProfessionalAndUserJoin = """
        SELECT
            sp.PK_SERVICE_PROFESSIONAL_ID AS Id,
            sp.PK_PROFESSIONAL_ID AS ProfessionalId,
            sp.PK_SERVICE_ID AS ServiceId,
            sp.CL_NEGOTIATED_PRICE AS NegotiatedPrice,
            sp.CL_ESTIMATED_ARRIVAL_MINUTES AS EstimatedArrivalMinutes,
            sp.CL_ARRIVAL_CONFIRMED_AT AS ArrivalConfirmedAt,
            sp.CL_STATUS AS Status,
            p.PK_PROFESSIONAL_ID AS Id,
            p.PK_USER_ID AS UserId,
            p.CL_DESCRIPTION AS Description,
            p.CL_EXPERIENCE AS Experience,
            p.CL_AVAILABILITY AS Availability,
            p.CL_LOCATION AS Location,
            p.CL_AVERAGE_RATING AS AverageRating,
            p.CL_COMPANY_USER_ID AS CompanyUserId,
            u.PK_USER_ID AS Id,
            u.CL_USERNAME AS Username,
            u.CL_FIRST_NAME AS FirstName,
            u.CL_LAST_NAME AS LastName,
            u.CL_EMAIL AS Email,
            u.CL_PHONE AS Phone,
            u.CL_PASSWORD AS PasswordHash,
            u.CL_ROLE AS Role,
            u.CL_STATUS AS Status,
            u.CL_REGISTRATION_DATE AS RegistrationDate
        FROM TBL_SERVICE_PROFESSIONALS sp
        INNER JOIN TBL_PROFESSIONALS p ON p.PK_PROFESSIONAL_ID = sp.PK_PROFESSIONAL_ID
        INNER JOIN TBL_USERS u ON u.PK_USER_ID = p.PK_USER_ID
        """;

    private static ServiceProfessional MapRow(ServiceProfessionalRow spRow, Professional professional, UserRow userRow)
    {
        professional.User = userRow.ToUser();
        var serviceProfessional = spRow.ToServiceProfessional();
        serviceProfessional.Professional = professional;
        return serviceProfessional;
    }

    public async Task<ServiceProfessional?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<ServiceProfessionalRow, Professional, UserRow, ServiceProfessional>(
            $"{SelectWithProfessionalAndUserJoin} WHERE sp.PK_SERVICE_PROFESSIONAL_ID = @Id",
            MapRow,
            new { Id = id },
            splitOn: "Id,Id");

        return results.FirstOrDefault();
    }

    public async Task<IReadOnlyList<ServiceProfessional>> GetByServiceIdAsync(int serviceId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<ServiceProfessionalRow, Professional, UserRow, ServiceProfessional>(
            $"{SelectWithProfessionalAndUserJoin} WHERE sp.PK_SERVICE_ID = @ServiceId",
            MapRow,
            new { ServiceId = serviceId },
            splitOn: "Id,Id");

        return results.ToList();
    }

    public async Task<ServiceProfessional?> GetAcceptedForServiceAsync(int serviceId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<ServiceProfessionalRow, Professional, UserRow, ServiceProfessional>(
            $"{SelectWithProfessionalAndUserJoin} WHERE sp.PK_SERVICE_ID = @ServiceId AND sp.CL_STATUS = 'ACCEPTED'",
            MapRow,
            new { ServiceId = serviceId },
            splitOn: "Id,Id");

        return results.FirstOrDefault();
    }

    // Distinct WorkType names this professional has an ACCEPTED, IN PROGRESS, or COMPLETED
    // proposal on — i.e. real jobs they've actually been hired for, not just bid on. Ordered
    // by the most recent proposal first, capped to `limit` (see FILE SUMMARY on the interface).
    public async Task<IReadOnlyList<string>> GetAcceptedWorkTypeNamesAsync(int professionalId, int limit)
    {
        using var connection = _connectionFactory.CreateConnection();
        var names = await connection.QueryAsync<string>(
            """
            SELECT DISTINCT TOP (@Limit) wt.CL_NAME
            FROM TBL_SERVICE_PROFESSIONALS sp
            INNER JOIN TBL_SERVICES s ON s.PK_SERVICE_ID = sp.PK_SERVICE_ID
            INNER JOIN TBL_WORKTYPES wt ON wt.PK_WORK_TYPE_ID = s.PK_WORK_TYPE_ID
            WHERE sp.PK_PROFESSIONAL_ID = @ProfessionalId
              AND sp.CL_STATUS IN ('ACCEPTED', 'IN PROGRESS', 'COMPLETED')
            ORDER BY wt.CL_NAME
            """,
            new { ProfessionalId = professionalId, Limit = limit });

        return names.ToList();
    }

    public async Task<bool> ExistsProposalAsync(int serviceId, int professionalId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM TBL_SERVICE_PROFESSIONALS
                WHERE PK_SERVICE_ID = @ServiceId AND PK_PROFESSIONAL_ID = @ProfessionalId
            ) THEN 1 ELSE 0 END
            """,
            new { ServiceId = serviceId, ProfessionalId = professionalId });
    }

    public async Task AddAsync(ServiceProfessional serviceProfessional)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_SERVICE_PROFESSIONALS
                (PK_PROFESSIONAL_ID, PK_SERVICE_ID, CL_NEGOTIATED_PRICE, CL_ESTIMATED_ARRIVAL_MINUTES, CL_STATUS)
            OUTPUT INSERTED.PK_SERVICE_PROFESSIONAL_ID AS Id
            VALUES (@ProfessionalId, @ServiceId, @NegotiatedPrice, @EstimatedArrivalMinutes, @Status)
            """;

        serviceProfessional.Id = await connection.ExecuteScalarAsync<int>(sql, new
        {
            serviceProfessional.ProfessionalId,
            serviceProfessional.ServiceId,
            serviceProfessional.NegotiatedPrice,
            serviceProfessional.EstimatedArrivalMinutes,
            Status = ServiceProfessionalStatusMapper.ToDb(serviceProfessional.Status)
        });
    }

    public async Task UpdateAsync(ServiceProfessional serviceProfessional)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(UpdateSql, ToUpdateParameters(serviceProfessional));
    }

    public async Task UpdateRangeAsync(IEnumerable<ServiceProfessional> serviceProfessionals)
    {
        using var connection = _connectionFactory.CreateConnection();
        // Dapper detects an IEnumerable<T> parameter and executes this SQL once per item —
        // no separate "batch" API needed for a handful of rows like this one.
        await connection.ExecuteAsync(UpdateSql, serviceProfessionals.Select(ToUpdateParameters));
    }

    private static object ToUpdateParameters(ServiceProfessional serviceProfessional) => new
    {
        serviceProfessional.Id,
        serviceProfessional.NegotiatedPrice,
        serviceProfessional.ArrivalConfirmedAt,
        Status = ServiceProfessionalStatusMapper.ToDb(serviceProfessional.Status)
    };

    private const string UpdateSql = """
        UPDATE TBL_SERVICE_PROFESSIONALS
        SET CL_NEGOTIATED_PRICE = @NegotiatedPrice,
            CL_ARRIVAL_CONFIRMED_AT = @ArrivalConfirmedAt,
            CL_STATUS = @Status
        WHERE PK_SERVICE_PROFESSIONAL_ID = @Id
        """;
}
