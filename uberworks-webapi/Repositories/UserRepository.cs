// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IUserRepository.cs — this is where Dapper runs
//               the actual SQL against TBL_USERS. Every method opens its own connection
//               (see Data/IDbConnectionFactory.cs) inside a `using` block, so it's always
//               closed/returned to the pool right after the query runs — nothing stays open
//               across requests. Queries select into UserRow.cs (CL_ROLE/CL_STATUS as plain
//               strings), then call .ToUser() to get the real User with UserRoleMapper.cs/
//               UserStatusMapper.cs doing the enum conversion — see UserRoleMapper.cs's FILE
//               SUMMARY for why Dapper can't be trusted to do that conversion by itself.
//               The same reasoning applies in reverse for AddAsync/UpdateAsync: Role/Status
//               are converted to their DB string explicitly in the parameters object,
//               instead of passing the User object (with its enum properties) directly to
//               Dapper.
// Entities connected: User.cs
// Tables related: TBL_USERS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // Every SELECT below shares this same column list, aliased to match UserRow.cs's
    // property names exactly (Dapper matches by name, case-insensitively).
    private const string SelectColumns = """
        SELECT
            PK_USER_ID AS Id,
            CL_USERNAME AS Username,
            CL_FIRST_NAME AS FirstName,
            CL_LAST_NAME AS LastName,
            CL_EMAIL AS Email,
            CL_PHONE AS Phone,
            CL_PASSWORD AS PasswordHash,
            CL_ROLE AS Role,
            CL_STATUS AS Status,
            CL_REGISTRATION_DATE AS RegistrationDate
        FROM TBL_USERS
        """;

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<UserRow>(
            $"{SelectColumns} WHERE PK_USER_ID = @Id", new { Id = id });

        return row?.ToUser();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<UserRow>(
            $"{SelectColumns} WHERE CL_EMAIL = @Email", new { Email = email });

        return row?.ToUser();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM TBL_USERS WHERE CL_EMAIL = @Email) THEN 1 ELSE 0 END",
            new { Email = email });
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM TBL_USERS WHERE CL_USERNAME = @Username) THEN 1 ELSE 0 END",
            new { Username = username });
    }

    public async Task AddAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        // OUTPUT INSERTED.<col> hands back the values SQL Server generated itself (the
        // identity PK_USER_ID, and CL_REGISTRATION_DATE's DEFAULT GETDATE()) in the same
        // round trip as the INSERT — no separate "SELECT SCOPE_IDENTITY()" call needed.
        const string sql = """
            INSERT INTO TBL_USERS (CL_USERNAME, CL_FIRST_NAME, CL_LAST_NAME, CL_EMAIL, CL_PHONE, CL_PASSWORD, CL_ROLE, CL_STATUS)
            OUTPUT INSERTED.PK_USER_ID AS Id, INSERTED.CL_REGISTRATION_DATE AS RegistrationDate
            VALUES (@Username, @FirstName, @LastName, @Email, @Phone, @PasswordHash, @Role, @Status)
            """;

        var generated = await connection.QuerySingleAsync(sql, new
        {
            user.Username,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.PasswordHash,
            Role = UserRoleMapper.ToDb(user.Role),
            Status = UserStatusMapper.ToDb(user.Status)
        });

        user.Id = (int)generated.Id;
        user.RegistrationDate = (DateTime)generated.RegistrationDate;
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE TBL_USERS
            SET CL_USERNAME = @Username,
                CL_FIRST_NAME = @FirstName,
                CL_LAST_NAME = @LastName,
                CL_EMAIL = @Email,
                CL_PHONE = @Phone,
                CL_PASSWORD = @PasswordHash,
                CL_ROLE = @Role,
                CL_STATUS = @Status
            WHERE PK_USER_ID = @Id
            """;

        await connection.ExecuteAsync(sql, new
        {
            user.Id,
            user.Username,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.PasswordHash,
            Role = UserRoleMapper.ToDb(user.Role),
            Status = UserStatusMapper.ToDb(user.Status)
        });
    }
}
