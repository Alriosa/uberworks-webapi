// =====================================================================================
// FILE SUMMARY
// What it does: One row per unhandled/unexpected error (HTTP 500) anywhere in the API.
//               Written automatically by Middleware/ExceptionHandlingMiddleware.cs — no
//               Controller or Service has to remember to log errors themselves, it happens
//               for the whole app in one place. Business exceptions that are expected and
//               already handled (NotFoundException → 404, ConflictException → 409, etc.)
//               do NOT land here — those are normal control flow, not "errors" for audit
//               purposes (a failed login attempt, for example, is recorded in
//               UserActionLog.cs as a LOGIN_FAILED action, not here).
// Entities connected: None (intentionally has no FK to User — see Username field note below)
// Tables related: TBL_ERROR_LOGS
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

public class ErrorLog
{
    public int Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public Common.Enums.LogSource Source { get; set; }

    /// <summary>
    /// Snapshot of who was logged in when the error happened (null if anonymous).
    /// Intentionally NOT a foreign key to User — if that account is ever deleted, this log
    /// row must survive untouched for audit purposes.
    /// </summary>
    public int? UserId { get; set; }
    public string Username { get; set; } = string.Empty;

    public string RequestMethod { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string ExceptionType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? IpAddress { get; set; }
}
