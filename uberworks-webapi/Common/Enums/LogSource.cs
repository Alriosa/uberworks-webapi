// =====================================================================================
// FILE SUMMARY
// What it does: Identifies HOW a request reached the API — from the future webapp, the
//               future mobile app, or "Direct" (Postman, curl, Swagger, or any client that
//               hasn't set the identifying header yet). This answers the "cómo" (how) part
//               of the audit requirement: every log row records not just who and when, but
//               also which client made the call. Determined by reading the "X-Client-Source"
//               request header (see Services/CurrentUserService.cs → Source).
// Entities connected: ErrorLog.cs, UserActionLog.cs, AdminActionLog.cs
// Tables related: TBL_ERROR_LOGS.CL_SOURCE, TBL_USER_ACTION_LOGS.CL_SOURCE,
//                 TBL_ADMIN_ACTION_LOGS.CL_SOURCE
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

public enum LogSource
{
    Direct,
    WebApp,
    MobileApp
}
