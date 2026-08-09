// =====================================================================================
// FILE SUMMARY
// What it does: Holds the dispute/report business logic. MapToResponseAsync resolves every
// human-readable name (client/professional/creator/resolver usernames, the related
// Service's description) via IUserRepository/IServiceRepository — N+1 lookups per report,
// accepted the same way ServiceService.GetAllForAdminAsync does, since this is a small
// internal admin/support listing, not a public endpoint. Images travel as a JSON array in
// Report.ImagesJson (see Report.cs) and are only ever serialized/deserialized here — nothing
// outside this Service ever sees raw JSON.
// GetByIdAsync auto-transitions a report from Open to Pending the first time its detail view
// is opened (per explicit request — "en espera significa que ya alguien abrió el caso, pero
// no lo ha terminado"); Resolved/Cancelled reports are left alone.
// ResolveAsync/NoFaultAsync/CancelAsync are honest about what they do NOT yet do: they
// persist the resolution/cancellation on the Report row itself (real, queryable, drives the
// Support dashboard's status buckets), but they do not deliver a message into a live chat
// inbox or move real money, because Chat.cs and Payment.cs have no Repository/Service/
// Controller built yet (see Task #70 in the project backlog) — that plumbing is a
// prerequisite this feature deliberately does not fake.
// Entities connected: Report.cs, Service.cs, User.cs
// Tables related: TBL_REPORTS, TBL_SERVICES, TBL_USERS (all indirectly, via IReportRepository/
//                 IServiceRepository/IUserRepository)
// =====================================================================================
using System.Text.Json;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;
    private readonly IServiceRepository _serviceRepository;

    public ReportService(IReportRepository reportRepository, IUserRepository userRepository, IServiceRepository serviceRepository)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<ReportResponse> CreateAsync(int createdByUserId, CreateReportRequest request, IReadOnlyList<string> imageUrls)
    {
        var report = new Report
        {
            Title = request.Title,
            Description = request.Description,
            ServiceId = request.ServiceId,
            ClientUserId = request.ClientUserId,
            ProfessionalUserId = request.ProfessionalUserId,
            CreatedByUserId = createdByUserId,
            IncidentDate = request.IncidentDate,
            ImagesJson = imageUrls.Count > 0 ? JsonSerializer.Serialize(imageUrls) : null
        };

        await _reportRepository.AddAsync(report);

        return await MapToResponseAsync(report);
    }

    public async Task<ReportResponse> CreateFromClientAsync(int clientUserId, ClientCreateReportRequest request, IReadOnlyList<string> imageUrls)
    {
        // A client can only tie their support case to one of THEIR OWN service requests —
        // never someone else's, even if they somehow guessed a valid id.
        if (request.ServiceId is int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId)
                ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

            if (service.ClientId != clientUserId)
            {
                throw new ForbiddenException("You can only file a support case about your own service requests.");
            }
        }

        var report = new Report
        {
            Title = request.Title,
            Description = request.Description,
            ServiceId = request.ServiceId,
            ClientUserId = clientUserId,
            CreatedByUserId = clientUserId,
            ImagesJson = imageUrls.Count > 0 ? JsonSerializer.Serialize(imageUrls) : null
        };

        await _reportRepository.AddAsync(report);

        return await MapToResponseAsync(report);
    }

    public async Task<IReadOnlyList<ReportResponse>> GetAllAsync()
    {
        var reports = await _reportRepository.GetAllAsync();
        var result = new List<ReportResponse>(reports.Count);

        foreach (var report in reports)
        {
            result.Add(await MapToResponseAsync(report));
        }

        return result;
    }

    // Only Support/Admin/MasterAdmin can reach this endpoint at all (ReportsController.cs's
    // class-level [Authorize]), so "someone called GetByIdAsync" already means "Support (or
    // an Admin standing in for Support) opened this specific report's detail view" — the
    // exact trigger the user asked for ("en espera significa que ya alguien abrió el caso").
    // A report already Pending/Resolved/Cancelled is left alone; only a brand-new Open one
    // flips.
    public async Task<ReportResponse> GetByIdAsync(int id)
    {
        var report = await GetOrThrowAsync(id);

        if (report.Status == ReportStatus.Open)
        {
            report.Status = ReportStatus.Pending;
            await _reportRepository.UpdateAsync(report);
        }

        return await MapToResponseAsync(report);
    }

    public async Task<ReportResponse> UpdateAsync(int id, UpdateReportRequest request)
    {
        var report = await GetOrThrowAsync(id);

        report.Title = request.Title;
        report.Description = request.Description;
        report.ServiceId = request.ServiceId;
        report.ClientUserId = request.ClientUserId;
        report.ProfessionalUserId = request.ProfessionalUserId;
        report.IncidentDate = request.IncidentDate;

        await _reportRepository.UpdateAsync(report);

        return await MapToResponseAsync(report);
    }

    public async Task<ReportResponse> ResolveAsync(int id, int resolvedByUserId, ResolveReportRequest request)
    {
        var report = await GetOrThrowAsync(id);

        report.Status = ReportStatus.Resolved;
        report.ResolutionMessage = request.Message;
        report.PaymentOutcome = request.PaymentOutcome;
        report.ResolvedByUserId = resolvedByUserId;
        report.ResolvedAt = DateTime.UtcNow;

        await _reportRepository.UpdateAsync(report);

        return await MapToResponseAsync(report);
    }

    public async Task<ReportResponse> NoFaultAsync(int id, int resolvedByUserId)
    {
        var report = await GetOrThrowAsync(id);

        report.Status = ReportStatus.Resolved;
        report.ResolutionMessage = "Closed with no fault assigned to either party. The payment hold was lifted and the normal payment flow proceeds.";
        report.PaymentOutcome = null;
        report.ResolvedByUserId = resolvedByUserId;
        report.ResolvedAt = DateTime.UtcNow;

        await _reportRepository.UpdateAsync(report);

        return await MapToResponseAsync(report);
    }

    public async Task<ReportResponse> CancelAsync(int id, int resolvedByUserId, CancelReportRequest request)
    {
        var report = await GetOrThrowAsync(id);

        report.Status = ReportStatus.Cancelled;
        report.CancellationReason = request.Reason;
        report.ResolvedByUserId = resolvedByUserId;
        report.ResolvedAt = DateTime.UtcNow;

        await _reportRepository.UpdateAsync(report);

        return await MapToResponseAsync(report);
    }

    private async Task<Report> GetOrThrowAsync(int id) =>
        await _reportRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Report with id {id} was not found.");

    private async Task<ReportResponse> MapToResponseAsync(Report report)
    {
        var client = report.ClientUserId is int clientId ? await _userRepository.GetByIdAsync(clientId) : null;
        var professional = report.ProfessionalUserId is int professionalId ? await _userRepository.GetByIdAsync(professionalId) : null;
        var createdBy = await _userRepository.GetByIdAsync(report.CreatedByUserId);
        var resolvedBy = report.ResolvedByUserId is int resolvedById ? await _userRepository.GetByIdAsync(resolvedById) : null;
        var service = report.ServiceId is int serviceId ? await _serviceRepository.GetByIdAsync(serviceId) : null;

        return new ReportResponse
        {
            Id = report.Id,
            Title = report.Title,
            Description = report.Description,
            ServiceId = report.ServiceId,
            ServiceDescription = service?.Description,
            ClientUserId = report.ClientUserId,
            ClientUsername = client?.Username,
            ProfessionalUserId = report.ProfessionalUserId,
            ProfessionalUsername = professional?.Username,
            CreatedByUserId = report.CreatedByUserId,
            CreatedByUsername = createdBy?.Username ?? string.Empty,
            IncidentDate = report.IncidentDate,
            CreatedAt = report.CreatedAt,
            Status = report.Status,
            Images = string.IsNullOrEmpty(report.ImagesJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(report.ImagesJson) ?? new List<string>(),
            ResolutionMessage = report.ResolutionMessage,
            PaymentOutcome = report.PaymentOutcome,
            CancellationReason = report.CancellationReason,
            ResolvedByUserId = report.ResolvedByUserId,
            ResolvedByUsername = resolvedBy?.Username,
            ResolvedAt = report.ResolvedAt
        };
    }
}
