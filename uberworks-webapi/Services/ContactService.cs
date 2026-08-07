// =====================================================================================
// FILE SUMMARY
// What it does: Business logic behind the "didn't find the service you needed?" form.
//               Reads the uploaded file (if any) into an EmailAttachment.cs, builds the
//               email body from the submitted fields, and sends it via IEmailSender to the
//               site owner's email (reuses MasterAdmin:Email from configuration — the same
//               address already used to seed the MasterAdmin account, so no separate
//               "Contact:NotificationEmail" setting is needed). Also logs the submission via
//               IAuditLogService so there's a permanent record even before real SMTP
//               credentials exist (see SmtpEmailSender.cs's log fallback).
// Entities connected: None directly
// Tables related: None directly (the audit log call reaches TBL_USER_ACTION_LOGS)
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class ContactService : IContactService
{
    // Keeps a malicious/oversized upload from ever reaching SmtpEmailSender.cs — 10 MB is
    // comfortably enough for a photo or a short document, without letting someone attach
    // something huge to an unauthenticated public form.
    private const long MaxAttachmentSizeBytes = 10 * 1024 * 1024;

    private readonly IEmailSender _emailSender;
    private readonly IAuditLogService _auditLogService;
    private readonly IConfiguration _configuration;

    public ContactService(IEmailSender emailSender, IAuditLogService auditLogService, IConfiguration configuration)
    {
        _emailSender = emailSender;
        _auditLogService = auditLogService;
        _configuration = configuration;
    }

    public async Task SuggestServiceAsync(ServiceSuggestionRequest request)
    {
        EmailAttachment? attachment = null;

        if (request.Attachment is not null)
        {
            if (request.Attachment.Length > MaxAttachmentSizeBytes)
            {
                throw new ArgumentException("The attached file is too large (max 10 MB).");
            }

            using var memoryStream = new MemoryStream();
            await request.Attachment.CopyToAsync(memoryStream);
            attachment = new EmailAttachment(request.Attachment.FileName, memoryStream.ToArray(), request.Attachment.ContentType);
        }

        var notificationEmail = _configuration["MasterAdmin:Email"]
            ?? throw new InvalidOperationException("MasterAdmin:Email is not configured in appsettings.");

        var companyLine = request.IsFromCompany
            ? $"<p><strong>Company:</strong> {request.CompanyName}</p>"
            : "<p><strong>Company:</strong> No (individual)</p>";

        await _emailSender.SendAsync(
            notificationEmail,
            $"New service suggestion from {request.Name}",
            $"""
             <p><strong>Name:</strong> {request.Name}</p>
             {companyLine}
             <p><strong>Reply-to email:</strong> {request.Email}</p>
             <p><strong>Message:</strong></p>
             <p>{request.Message}</p>
             """,
            attachment);

        await _auditLogService.LogUserActionAsync(
            actorUserId: null,
            actorUsername: request.Name,
            action: "SERVICE_SUGGESTED",
            targetEntityType: null,
            targetEntityId: null,
            details: $"Email={request.Email}, IsFromCompany={request.IsFromCompany}, CompanyName={request.CompanyName}, HasAttachment={attachment is not null}");
    }
}
