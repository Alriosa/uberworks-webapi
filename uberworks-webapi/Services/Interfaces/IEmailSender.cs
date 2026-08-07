// =====================================================================================
// FILE SUMMARY
// What it does: Contract for sending an email. UserService.cs/ContactService.cs depend on
//               this interface, not on SmtpEmailSender.cs directly, so the real delivery
//               mechanism can be swapped later (a transactional-email API instead of raw
//               SMTP, for example) without touching business logic. The attachment parameter
//               is optional (null for the password-reset email, set for the "suggest a
//               service" contact form) — see EmailAttachment.cs.
// Entities connected: None
// Tables related: None
// =====================================================================================
using uberworks_webapi.Services;

namespace uberworks_webapi.Services.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody, EmailAttachment? attachment = null);
}
