// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IServiceRepository.cs. Uses Dapper
//               multi-mapping (same idea as ProfessionalRepository.cs) to fill both a
//               Service and its related WorkType from one JOIN query — this is what EF
//               Core's ".Include(s => s.WorkType)" used to do. Client is intentionally NOT
//               joined here (matches the original behavior) — nothing currently needs the
//               full Client object alongside a Service, only its Id. Maps into the private
//               ServiceRow (CL_STATUS as a plain string) before converting to the real
//               Service via ServiceStatusMapper.FromDb — see UserRoleMapper.cs's FILE
//               SUMMARY for why Dapper can't be trusted to convert that column by itself.
// Entities connected: Service.cs, WorkType.cs (via a real SQL JOIN)
// Tables related: TBL_SERVICES, TBL_WORKTYPES
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Data;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ServiceRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // The raw shape of a joined Service+WorkType row — CL_STATUS stays a string here (see
    // FILE SUMMARY); ToService() converts it into the real Service with its WorkType attached.
    private class ServiceRow
    {
        public int Id { get; set; }
        public int WorkTypeId { get; set; }
        public int ClientId { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? ProposedPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string ExactAddress { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public string? CompletionPhotoUrl { get; set; }
        public DateTime? ClientConfirmedCompletionAt { get; set; }
        public DateTime? ProfessionalConfirmedCompletionAt { get; set; }

        public Service ToService(WorkType workType) => new()
        {
            Id = Id,
            WorkTypeId = WorkTypeId,
            ClientId = ClientId,
            Description = Description,
            ImageUrl = ImageUrl,
            ProposedPrice = ProposedPrice,
            Status = ServiceStatusMapper.FromDb(Status),
            RequestDate = RequestDate,
            Latitude = Latitude,
            Longitude = Longitude,
            ExactAddress = ExactAddress,
            Zone = Zone,
            CompletionPhotoUrl = CompletionPhotoUrl,
            ClientConfirmedCompletionAt = ClientConfirmedCompletionAt,
            ProfessionalConfirmedCompletionAt = ProfessionalConfirmedCompletionAt,
            WorkType = workType
        };
    }

    // s.* first (ServiceRow), then w.* (the joined WorkType) — order must match
    // <ServiceRow, WorkType, Service> in every Query<> call below.
    private const string SelectWithWorkTypeJoin = """
        SELECT
            s.PK_SERVICE_ID AS Id,
            s.PK_WORK_TYPE_ID AS WorkTypeId,
            s.CL_CLIENT_ID AS ClientId,
            s.CL_DESCRIPTION AS Description,
            s.CL_IMAGE_URL AS ImageUrl,
            s.CL_PROPOSED_PRICE AS ProposedPrice,
            s.CL_STATUS AS Status,
            s.CL_REQUEST_DATE AS RequestDate,
            s.CL_LATITUDE AS Latitude,
            s.CL_LONGITUDE AS Longitude,
            s.CL_EXACT_ADDRESS AS ExactAddress,
            s.CL_ZONE AS Zone,
            s.CL_COMPLETION_PHOTO_URL AS CompletionPhotoUrl,
            s.CL_CLIENT_CONFIRMED_AT AS ClientConfirmedCompletionAt,
            s.CL_PROFESSIONAL_CONFIRMED_AT AS ProfessionalConfirmedCompletionAt,
            w.PK_WORK_TYPE_ID AS Id,
            w.CL_NAME AS Name,
            w.CL_DESCRIPTION AS Description,
            w.CL_INCLUDES AS Includes,
            w.CL_NOT_INCLUDES AS NotIncludes
        FROM TBL_SERVICES s
        INNER JOIN TBL_WORKTYPES w ON w.PK_WORK_TYPE_ID = s.PK_WORK_TYPE_ID
        """;

    private static Service MapRow(ServiceRow row, WorkType workType) => row.ToService(workType);

    public async Task<Service?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<ServiceRow, WorkType, Service>(
            $"{SelectWithWorkTypeJoin} WHERE s.PK_SERVICE_ID = @Id",
            MapRow,
            new { Id = id },
            splitOn: "Id");

        return results.FirstOrDefault();
    }

    public async Task<IReadOnlyList<Service>> GetOpenAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<ServiceRow, WorkType, Service>(
            $"{SelectWithWorkTypeJoin} WHERE s.CL_STATUS = 'PENDING' ORDER BY s.CL_REQUEST_DATE DESC",
            MapRow,
            splitOn: "Id");

        return results.ToList();
    }

    public async Task<IReadOnlyList<Service>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<ServiceRow, WorkType, Service>(
            $"{SelectWithWorkTypeJoin} ORDER BY s.CL_REQUEST_DATE DESC",
            MapRow,
            splitOn: "Id");

        return results.ToList();
    }

    public async Task<IReadOnlyList<Service>> GetByClientIdAsync(int clientId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<ServiceRow, WorkType, Service>(
            $"{SelectWithWorkTypeJoin} WHERE s.CL_CLIENT_ID = @ClientId ORDER BY s.CL_REQUEST_DATE DESC",
            MapRow,
            new { ClientId = clientId },
            splitOn: "Id");

        return results.ToList();
    }

    public async Task AddAsync(Service service)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_SERVICES (
                PK_WORK_TYPE_ID, CL_CLIENT_ID, CL_DESCRIPTION, CL_IMAGE_URL, CL_PROPOSED_PRICE, CL_STATUS,
                CL_LATITUDE, CL_LONGITUDE, CL_EXACT_ADDRESS, CL_ZONE, CL_COMPLETION_PHOTO_URL,
                CL_CLIENT_CONFIRMED_AT, CL_PROFESSIONAL_CONFIRMED_AT)
            OUTPUT INSERTED.PK_SERVICE_ID AS Id, INSERTED.CL_REQUEST_DATE AS RequestDate
            VALUES (
                @WorkTypeId, @ClientId, @Description, @ImageUrl, @ProposedPrice, @Status,
                @Latitude, @Longitude, @ExactAddress, @Zone, @CompletionPhotoUrl,
                @ClientConfirmedCompletionAt, @ProfessionalConfirmedCompletionAt)
            """;

        var generated = await connection.QuerySingleAsync(sql, new
        {
            service.WorkTypeId,
            service.ClientId,
            service.Description,
            service.ImageUrl,
            service.ProposedPrice,
            Status = ServiceStatusMapper.ToDb(service.Status),
            service.Latitude,
            service.Longitude,
            service.ExactAddress,
            service.Zone,
            service.CompletionPhotoUrl,
            service.ClientConfirmedCompletionAt,
            service.ProfessionalConfirmedCompletionAt
        });

        service.Id = (int)generated.Id;
        service.RequestDate = (DateTime)generated.RequestDate;
    }

    public async Task UpdateAsync(Service service)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE TBL_SERVICES
            SET CL_DESCRIPTION = @Description,
                CL_IMAGE_URL = @ImageUrl,
                CL_PROPOSED_PRICE = @ProposedPrice,
                CL_STATUS = @Status,
                CL_COMPLETION_PHOTO_URL = @CompletionPhotoUrl,
                CL_CLIENT_CONFIRMED_AT = @ClientConfirmedCompletionAt,
                CL_PROFESSIONAL_CONFIRMED_AT = @ProfessionalConfirmedCompletionAt
            WHERE PK_SERVICE_ID = @Id
            """;

        await connection.ExecuteAsync(sql, new
        {
            service.Id,
            service.Description,
            service.ImageUrl,
            service.ProposedPrice,
            Status = ServiceStatusMapper.ToDb(service.Status),
            service.CompletionPhotoUrl,
            service.ClientConfirmedCompletionAt,
            service.ProfessionalConfirmedCompletionAt
        });
    }
}
