// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IPenaltyRepository.cs — Dapper runs the actual
//               SQL against TBL_PENALTIES. No JOINs: PenaltyService.cs resolves the
//               human-readable username itself via IUserRepository, same N+1-is-fine
//               reasoning as ReportRepository.cs/ServiceService.cs. CL_TYPE goes through
//               PenaltyTypeMapper explicitly — see UserRoleMapper.cs's FILE SUMMARY for why
//               Dapper can't be trusted to convert that column by itself.
// Entities connected: Penalty.cs
// Tables related: TBL_PENALTIES
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class PenaltyRepository : IPenaltyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PenaltyRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // Raw row shape — CL_TYPE stays a string here (see FILE SUMMARY); ToPenalty() converts
    // it into the real Penalty.
    private class PenaltyRow
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Penalty ToPenalty() => new()
        {
            Id = Id,
            UserId = UserId,
            Type = PenaltyTypeMapper.FromDb(Type),
            Reason = Reason,
            StartDate = StartDate,
            EndDate = EndDate
        };
    }

    private const string SelectColumns = """
        SELECT
            PK_PENALTY_ID AS Id,
            PK_USER_ID AS UserId,
            CL_TYPE AS Type,
            CL_REASON AS Reason,
            CL_START_DATE AS StartDate,
            CL_END_DATE AS EndDate
        FROM TBL_PENALTIES
        """;

    public async Task<IReadOnlyList<Penalty>> GetByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<PenaltyRow>(
            $"{SelectColumns} WHERE PK_USER_ID = @UserId ORDER BY CL_START_DATE DESC", new { UserId = userId });

        return rows.Select(row => row.ToPenalty()).ToList();
    }

    public async Task<IReadOnlyList<Penalty>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<PenaltyRow>($"{SelectColumns} ORDER BY CL_START_DATE DESC");

        return rows.Select(row => row.ToPenalty()).ToList();
    }

    public async Task AddAsync(Penalty penalty)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_PENALTIES (PK_USER_ID, CL_TYPE, CL_REASON, CL_START_DATE, CL_END_DATE)
            OUTPUT INSERTED.PK_PENALTY_ID AS Id
            VALUES (@UserId, @Type, @Reason, @StartDate, @EndDate)
            """;

        var generatedId = await connection.QuerySingleAsync<int>(sql, new
        {
            penalty.UserId,
            Type = PenaltyTypeMapper.ToDb(penalty.Type),
            penalty.Reason,
            penalty.StartDate,
            penalty.EndDate
        });

        penalty.Id = generatedId;
    }
}
