// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IProfessionalRepository.cs, using Dapper's
//               "multi-mapping" to fill both a Professional AND its related User from one
//               single SQL query with a JOIN (this is what EF Core's ".Include(p => p.User)"
//               used to do automatically). Maps into UserRow.cs (not User.cs directly) for
//               the joined columns, then calls .ToUser() — see UserRoleMapper.cs for why.
//               splitOn: "Id" tells Dapper where the columns for the second object start in
//               the row — both TBL_PROFESSIONALS and TBL_USERS have their own "Id" column,
//               so Dapper needs to be told explicitly where one ends and the other begins;
//               it looks for the NEXT column named "Id" after the first one, not the very
//               first column, so this works correctly even though Professional's own Id is
//               also called "Id".
// Entities connected: Professional.cs, User.cs (via a real SQL JOIN)
// Tables related: TBL_PROFESSIONALS, TBL_USERS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class ProfessionalRepository : IProfessionalRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProfessionalRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // p.* first (maps to the Professional object), then u.* (maps to the joined UserRow)
    // — the order here must match the <Professional, UserRow, Professional> order used in
    // every Query<> call below.
    private const string SelectWithUserJoin = """
        SELECT
            p.PK_PROFESSIONAL_ID AS Id,
            p.PK_USER_ID AS UserId,
            p.CL_DESCRIPTION AS Description,
            p.CL_EXPERIENCE AS Experience,
            p.CL_AVAILABILITY AS Availability,
            p.CL_LOCATION AS Location,
            p.CL_AVERAGE_RATING AS AverageRating,
            p.CL_COMPANY_USER_ID AS CompanyUserId,
            p.CL_PHOTO_URL AS PhotoUrl,
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
        FROM TBL_PROFESSIONALS p
        INNER JOIN TBL_USERS u ON u.PK_USER_ID = p.PK_USER_ID
        """;

    private static Professional MapWithUser(Professional professional, UserRow userRow)
    {
        professional.User = userRow.ToUser();
        return professional;
    }

    public async Task<Professional?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Professional, UserRow, Professional>(
            $"{SelectWithUserJoin} WHERE p.PK_PROFESSIONAL_ID = @Id",
            MapWithUser,
            new { Id = id },
            splitOn: "Id");

        return results.FirstOrDefault();
    }

    public async Task<Professional?> GetByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Professional, UserRow, Professional>(
            $"{SelectWithUserJoin} WHERE p.PK_USER_ID = @UserId",
            MapWithUser,
            new { UserId = userId },
            splitOn: "Id");

        return results.FirstOrDefault();
    }

    public async Task<bool> ExistsByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM TBL_PROFESSIONALS WHERE PK_USER_ID = @UserId) THEN 1 ELSE 0 END",
            new { UserId = userId });
    }

    public async Task<List<Professional>> GetByCompanyUserIdAsync(int companyUserId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Professional, UserRow, Professional>(
            $"{SelectWithUserJoin} WHERE p.CL_COMPANY_USER_ID = @CompanyUserId",
            MapWithUser,
            new { CompanyUserId = companyUserId },
            splitOn: "Id");

        return results.ToList();
    }

    public async Task AddAsync(Professional professional)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_PROFESSIONALS (PK_USER_ID, CL_DESCRIPTION, CL_EXPERIENCE, CL_AVAILABILITY, CL_LOCATION, CL_COMPANY_USER_ID, CL_PHOTO_URL)
            OUTPUT INSERTED.PK_PROFESSIONAL_ID AS Id, INSERTED.CL_AVERAGE_RATING AS AverageRating
            VALUES (@UserId, @Description, @Experience, @Availability, @Location, @CompanyUserId, @PhotoUrl)
            """;

        var generated = await connection.QuerySingleAsync(sql, new
        {
            professional.UserId,
            professional.Description,
            professional.Experience,
            professional.Availability,
            professional.Location,
            professional.CompanyUserId,
            professional.PhotoUrl
        });

        professional.Id = (int)generated.Id;
        professional.AverageRating = (decimal)generated.AverageRating;
    }

    public async Task UpdateAsync(Professional professional)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE TBL_PROFESSIONALS
            SET CL_DESCRIPTION = @Description,
                CL_EXPERIENCE = @Experience,
                CL_AVAILABILITY = @Availability,
                CL_LOCATION = @Location,
                CL_COMPANY_USER_ID = @CompanyUserId,
                CL_PHOTO_URL = @PhotoUrl
            WHERE PK_PROFESSIONAL_ID = @Id
            """;

        await connection.ExecuteAsync(sql, new
        {
            professional.Id,
            professional.Description,
            professional.Experience,
            professional.Availability,
            professional.Location,
            professional.CompanyUserId,
            professional.PhotoUrl
        });
    }
}
