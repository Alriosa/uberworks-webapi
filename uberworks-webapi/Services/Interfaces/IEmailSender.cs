// =====================================================================================
// FILE SUMMARY
// What it does: Contract for sending an email. UserService.cs depends on this interface,
//               not on SmtpEmailSender.cs directly, so the real delivery mechanism can be
//               swapped later (a transactional-email API instead of raw SMTP, for example)
//               without touching business logic.
// Entities connected: None
// Tables related: None
// =====================================================================================
namespace uberworks_webapi.Services.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}
