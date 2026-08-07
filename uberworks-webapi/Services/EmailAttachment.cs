// =====================================================================================
// FILE SUMMARY
// What it does: A file to attach to an outgoing email — just the raw bytes, the file name,
//               and its content type (e.g. "image/png"). Kept as plain bytes (not an
//               IFormFile) so IEmailSender.cs doesn't need to know anything about
//               ASP.NET Core's HTTP upload types — a Controller reads the uploaded file into
//               this shape once, up front, and everything downstream (ContactService.cs,
//               SmtpEmailSender.cs) just deals with bytes.
// Entities connected: None
// Tables related: None
// =====================================================================================
namespace uberworks_webapi.Services;

public record EmailAttachment(string FileName, byte[] Content, string ContentType);
