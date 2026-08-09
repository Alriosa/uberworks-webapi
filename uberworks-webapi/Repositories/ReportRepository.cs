// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IReportRepository.cs — Dapper runs the actual
// SQL against TBL_REPORTS. No JOINs here (unlike ProfessionalRepository.cs/
// ServiceRepository.cs): Service/Client/Professional/CreatedBy/ResolvedBy are all just
// nullable ids on this entity — ReportService.cs resolves the human-readable names itself
// via IUserRepository/IServiceRepository, same N+1-is-fine-for-an-admin-listing reasoning as
// ServiceService.GetAllForAdminAsync. CL_STATUS/CL_PAYMENT_OUTCOME go through
// ReportStatusMapper/ReportPaymentOutcomeMapper explicitly — see UserRoleMapper.cs's FILE
// SUMMARY for why Dapper can't be trusted to convert those columns by itself.
// Entities connected: Report.cs
// Tables related: TBL_REPORTS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReportRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // Raw row shape — CL_STATUS/CL_PAYMENT_OUTCOME stay strings here (see FILE SUMMARY);
    // ToReport() converts them into the real Report.
    private class ReportRow
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? ServiceId { get; set; }
        public int? ClientUserId { get; set; }
        public int? ProfessionalUserId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? IncidentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ImagesJson { get; set; }
        public string? ResolutionMessage { get; set; }
        public string? PaymentOutcome { get; set; }
        public string? CancellationReason { get; set; }
        public int? ResolvedByUserId { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public Report ToReport() => new()
        {
            Id = Id,
            Title = Title,
            Description = Description,
            ServiceId = ServiceId,
            ClientUserId = ClientUserId,
            ProfessionalUserId = ProfessionalUserId,
            CreatedByUserId = CreatedByUserId,
            IncidentDate = IncidentDate,
            CreatedAt = CreatedAt,
            Status = ReportStatusMapper.FromDb(Status),
            ImagesJson = ImagesJson,
            ResolutionMessage = ResolutionMessage,
            PaymentOutcome = ReportPaymentOutcomeMapper.FromDb(PaymentOutcome),
            CancellationReason = CancellationReason,
            ResolvedByUserId = ResolvedByUserId,
            ResolvedAt = ResolvedAt
        };
    }

    private const string SelectColumns = """
        SELECT
            PK_REPORT_ID AS Id,
            CL_TITLE AS Title,
            CL_DESCRIPTION AS Description,
            PK_SERVICE_ID AS ServiceId,
            CL_CLIENT_USER_ID AS ClientUserId,
            CL_PROFESSIONAL_USER_ID AS ProfessionalUserId,
            CL_CREATED_BY_USER_ID AS CreatedByUserId,
            CL_INCIDENT_DATE AS IncidentDate,
            CL_CREATED_AT AS CreatedAt,
            CL_STATUS AS Status,
            CL_IMAGES_JSON AS ImagesJson,
            CL_RESOLUTION_MESSAGE AS ResolutionMessage,
            CL_PAYMENT_OUTCOME AS PaymentOutcome,
            CL_CANCELLATION_REASON AS CancellationReason,
            CL_RESOLVED_BY_USER_ID AS ResolvedByUserId,
            CL_RESOLVED_AT AS ResolvedAt
        FROM TBL_REPORTS
        """;

    public async Task<Report?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<ReportRow>(
            $"{SelectColumns} WHERE PK_REPORT_ID = @Id", new { Id = id });

        return row?.ToReport();
    }

    public async Task<IReadOnlyList<Report>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ReportRow>($"{SelectColumns} ORDER BY CL_CREATED_AT DESC");

        return rows.Select(row => row.ToReport()).ToList();
    }

    public async Task AddAsync(Report report)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_REPORTS (
                CL_TITLE, CL_DESCRIPTION, PK_SERVICE_ID, CL_CLIENT_USER_ID, CL_PROFESSIONAL_USER_ID,
                CL_CREATED_BY_USER_ID, CL_INCIDENT_DATE, CL_STATUS, CL_IMAGES_JSON)
            OUTPUT INSERTED.PK_REPORT_ID AS Id, INSERTED.CL_CREATED_AT AS CreatedAt
            VALUES (
                @Title, @Description, @ServiceId, @ClientUserId, @ProfessionalUserId,
                @CreatedByUserId, @IncidentDate, @Status, @ImagesJson)
            """;

        var generated = await connection.QuerySingleAsync(sql, new
        {
            report.Title,
            report.Description,
            report.ServiceId,
            report.ClientUserId,
            report.ProfessionalUserId,
            report.CreatedByUserId,
            report.IncidentDate,
            Status = ReportStatusMapper.ToDb(report.Status),
            report.ImagesJson
        });

        report.Id = (int)generated.Id;
        report.CreatedAt = (DateTime)generated.CreatedAt;
    }

    public async Task UpdateAsync(Report report)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE TBL_REPORTS
            SET CL_TITLE = @Title,
                CL_DESCRIPTION = @Description,
                PK_SERVICE_ID = @ServiceId,
                CL_CLIENT_USER_ID = @ClientUserId,
                CL_PROFESSIONAL_USER_ID = @ProfessionalUserId,
                CL_INCIDENT_DATE = @IncidentDate,
                CL_STATUS = @Status,
                CL_IMAGES_JSON = @ImagesJson,
                CL_RESOLUTION_MESSAGE = @ResolutionMessage,
                CL_PAYMENT_OUTCOME = @PaymentOutcome,
                CL_CANCELLATION_REASON = @CancellationReason,
                CL_RESOLVED_BY_USER_ID = @ResolvedByUserId,
                CL_RESOLVED_AT = @ResolvedAt
            WHERE PK_REPORT_ID = @Id
            """;

        await connection.ExecuteAsync(sql, new
        {
            report.Id,
            report.Title,
            report.Description,
            report.ServiceId,
            report.ClientUserId,
            report.ProfessionalUserId,
            report.IncidentDate,
            Status = ReportStatusMapper.ToDb(report.Status),
            report.ImagesJson,
            report.ResolutionMessage,
            PaymentOutcome = ReportPaymentOutcomeMapper.ToDb(report.PaymentOutcome),
            report.CancellationReason,
            report.ResolvedByUserId,
            report.ResolvedAt
        });
    }
}
