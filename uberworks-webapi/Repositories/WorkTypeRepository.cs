// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IWorkTypeRepository.cs. HasServicesAsync() is
//               the query WorkTypeService.cs uses to block deleting a category that's
//               already used by some Service (which would leave orphaned Services) — it
//               queries TBL_SERVICES directly, a different table than the rest of this
//               class, which is why it doesn't reuse SelectColumns below.
// Entities connected: WorkType.cs, Service.cs (to check dependencies before deleting)
// Tables related: TBL_WORKTYPES, TBL_SERVICES
// =====================================================================================
using Dapper;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class WorkTypeRepository : IWorkTypeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public WorkTypeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private const string SelectColumns = """
        SELECT
            PK_WORK_TYPE_ID AS Id,
            CL_NAME AS Name,
            CL_DESCRIPTION AS Description,
            CL_INCLUDES AS Includes,
            CL_NOT_INCLUDES AS NotIncludes
        FROM TBL_WORKTYPES
        """;

    public async Task<WorkType?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<WorkType>(
            $"{SelectColumns} WHERE PK_WORK_TYPE_ID = @Id", new { Id = id });
    }

    public async Task<IReadOnlyList<WorkType>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<WorkType>($"{SelectColumns} ORDER BY CL_NAME");
        return results.ToList();
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM TBL_WORKTYPES
                WHERE CL_NAME = @Name AND (@ExcludeId IS NULL OR PK_WORK_TYPE_ID <> @ExcludeId)
            ) THEN 1 ELSE 0 END
            """,
            new { Name = name, ExcludeId = excludeId });
    }

    public async Task<bool> HasServicesAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM TBL_SERVICES WHERE PK_WORK_TYPE_ID = @Id) THEN 1 ELSE 0 END",
            new { Id = id });
    }

    public async Task AddAsync(WorkType workType)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_WORKTYPES (CL_NAME, CL_DESCRIPTION, CL_INCLUDES, CL_NOT_INCLUDES)
            OUTPUT INSERTED.PK_WORK_TYPE_ID AS Id
            VALUES (@Name, @Description, @Includes, @NotIncludes)
            """;

        workType.Id = await connection.ExecuteScalarAsync<int>(sql, workType);
    }

    public async Task UpdateAsync(WorkType workType)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE TBL_WORKTYPES
            SET CL_NAME = @Name,
                CL_DESCRIPTION = @Description,
                CL_INCLUDES = @Includes,
                CL_NOT_INCLUDES = @NotIncludes
            WHERE PK_WORK_TYPE_ID = @Id
            """;

        await connection.ExecuteAsync(sql, workType);
    }

    public async Task DeleteAsync(WorkType workType)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "DELETE FROM TBL_WORKTYPES WHERE PK_WORK_TYPE_ID = @Id", new { workType.Id });
    }
}
