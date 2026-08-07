// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IPasswordResetTokenRepository.cs.
//               GetValidByTokenHashAsync filters by Used = 0 AND ExpiresAt > GETUTCDATE()
//               directly in the SQL WHERE clause — an expired or already-used token simply
//               doesn't come back, same as if it never existed. Also joins TBL_USERS (via
//               Dapper multi-mapping into UserRow.cs, same pattern as
//               ProfessionalRepository.cs) since UserService.ResetPasswordAsync needs the
//               full User to update its password.
// Entities connected: PasswordResetToken.cs, User.cs (via a real SQL JOIN)
// Tables related: TBL_PASSWORD_RESET_TOKENS, TBL_USERS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PasswordResetTokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PasswordResetToken?> GetValidByTokenHashAsync(string tokenHash)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                t.PK_TOKEN_ID AS Id,
                t.PK_USER_ID AS UserId,
                t.CL_TOKEN_HASH AS TokenHash,
                t.CL_EXPIRES_AT AS ExpiresAt,
                t.CL_USED AS Used,
                t.CL_CREATED_AT AS CreatedAt,
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
            FROM TBL_PASSWORD_RESET_TOKENS t
            INNER JOIN TBL_USERS u ON u.PK_USER_ID = t.PK_USER_ID
            WHERE t.CL_TOKEN_HASH = @TokenHash AND t.CL_USED = 0 AND t.CL_EXPIRES_AT > GETUTCDATE()
            """;

        var results = await connection.QueryAsync<PasswordResetToken, UserRow, PasswordResetToken>(
            sql,
            (token, userRow) => { token.User = userRow.ToUser(); return token; },
            new { TokenHash = tokenHash },
            splitOn: "Id");

        return results.FirstOrDefault();
    }

    public async Task AddAsync(PasswordResetToken token)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_PASSWORD_RESET_TOKENS (PK_USER_ID, CL_TOKEN_HASH, CL_EXPIRES_AT)
            OUTPUT INSERTED.PK_TOKEN_ID AS Id, INSERTED.CL_CREATED_AT AS CreatedAt
            VALUES (@UserId, @TokenHash, @ExpiresAt)
            """;

        var generated = await connection.QuerySingleAsync(sql, new { token.UserId, token.TokenHash, token.ExpiresAt });
        token.Id = (int)generated.Id;
        token.CreatedAt = (DateTime)generated.CreatedAt;
    }

    public async Task UpdateAsync(PasswordResetToken token)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE TBL_PASSWORD_RESET_TOKENS SET CL_USED = @Used WHERE PK_TOKEN_ID = @Id",
            new { token.Id, token.Used });
    }
}
