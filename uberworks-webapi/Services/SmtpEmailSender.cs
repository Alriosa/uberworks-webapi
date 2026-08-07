// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IEmailSender.cs, using .NET's built-in
//               System.Net.Mail.SmtpClient (no external NuGet package needed — works fine
//               with Gmail's SMTP, SendGrid's SMTP relay, etc.). If Smtp:Host isn't
//               configured yet (appsettings/user-secrets), it does NOT throw or crash the
//               app — it just logs the email's subject/body (and the attachment's file name/
//               size, if any) instead of sending it. This is what lets the whole
//               forgot-password flow AND the "suggest a service" contact form be built and
//               tested end-to-end before real SMTP credentials exist; once they're added to
//               user-secrets, sending switches on automatically, no code changes needed.
// Entities connected: None
// Tables related: None
// =====================================================================================
using System.Net;
using System.Net.Mail;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody, EmailAttachment? attachment = null)
    {
        var smtpSection = _configuration.GetSection("Smtp");
        var host = smtpSection["Host"];

        if (string.IsNullOrWhiteSpace(host))
        {
            // No SMTP configured yet — log instead of sending, so the flow that TRIGGERS
            // the email (forgot-password, "suggest a service") can still be developed/tested.
            var attachmentNote = attachment is null
                ? "none"
                : $"{attachment.FileName} ({attachment.Content.Length} bytes)";

            _logger.LogWarning(
                "Smtp:Host is not configured — email NOT sent. To: {ToEmail}, Subject: {Subject}, Attachment: {Attachment}, Body: {Body}",
                toEmail, subject, attachmentNote, htmlBody);
            return;
        }

        var port = int.Parse(smtpSection["Port"] ?? "587");
        var username = smtpSection["Username"] ?? string.Empty;
        var password = smtpSection["Password"] ?? string.Empty;
        var fromEmail = smtpSection["FromEmail"] ?? username;
        var fromName = smtpSection["FromName"] ?? "Uberworks";
        var enableSsl = bool.Parse(smtpSection["EnableSsl"] ?? "true");

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        // The MemoryStream/Attachment objects must stay alive until after SendMailAsync
        // finishes reading from them — disposed by the `using` here, right after sending.
        using var attachmentStream = attachment is null ? null : new MemoryStream(attachment.Content);
        if (attachment is not null && attachmentStream is not null)
        {
            message.Attachments.Add(new Attachment(attachmentStream, attachment.FileName, attachment.ContentType));
        }

        await client.SendMailAsync(message);
    }
}
