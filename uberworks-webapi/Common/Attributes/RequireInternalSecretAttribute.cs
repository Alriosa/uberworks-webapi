// =====================================================================================
// FILE SUMMARY
// What it does: Guards an endpoint that has NO caller JWT to check, because the caller
//               (uberworks-webapp) is vouching for someone's identity on their behalf —
//               see Controllers/UsersController.cs -> ExternalLogin. Instead of
//               [Authorize], this checks a shared secret sent in the "X-Internal-Secret"
//               header against Internal:SharedSecret in configuration. Only trusted
//               internal clients (the WebApp, and later Mobile) know this value; it's
//               never sent to a browser or a public caller. If it's missing or wrong,
//               the request is rejected with 401 before the Controller action runs.
// Entities connected: None
// Tables related: None
// =====================================================================================
using Microsoft.AspNetCore.Mvc.Filters;

namespace uberworks_webapi.Common.Attributes;

public class RequireInternalSecretAttribute : Attribute, IAsyncActionFilter
{
    private const string HeaderName = "X-Internal-Secret";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expectedSecret = configuration["Internal:SharedSecret"];

        var providedSecret = context.HttpContext.Request.Headers[HeaderName].ToString();

        if (string.IsNullOrEmpty(expectedSecret) || providedSecret != expectedSecret)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
            return;
        }

        await next();
    }
}
