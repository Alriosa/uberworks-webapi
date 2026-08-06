// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the HTTP endpoints for registration, login, lookup, and update of
//               users. The Controller only receives/validates shape and delegates all real
//               logic to IUserService — it never decides business rules or touches the
//               database. GetById/Update require [Authorize] and pass the caller's identity
//               down to the Service, which enforces that only the profile owner or an
//               Admin/MasterAdmin can view/edit it (see UserService.EnsureSelfOrAdmin) —
//               this is what stops anyone from scraping every user's email/phone by id.
//               AdminCreate requires [Authorize(Roles = "MasterAdmin,Admin,Manager")] — a
//               Client/Professional/Company JWT gets a 403 from ASP.NET Core before the
//               action even runs — and which roles the caller can actually create from
//               there is enforced by UserService.CreateByAdminAsync's account-creation
//               pyramid (see UserRole.cs), not by this Controller.
//               ExternalLogin backs Google sign-in and is guarded by [RequireInternalSecret]
//               instead of [Authorize], since the caller doesn't have a JWT yet.
//               ForgotPassword/ResetPassword are public on purpose — they're exactly for
//               people who can't log in.
// Entities connected: User.cs (indirectly, via IUserService)
// Tables related: TBL_USERS (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Common.Attributes;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(IUserService userService, ICurrentUserService currentUserService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Backs Google sign-in. No [Authorize] here — there is no JWT yet, that's the whole
    /// point of the call. Instead, [RequireInternalSecret] checks that the caller is a
    /// trusted internal client (the WebApp), which already verified the email with Google
    /// before making this request.
    /// </summary>
    [HttpPost("external-login")]
    [RequireInternalSecret]
    public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginRequest request)
    {
        var result = await _userService.ExternalLoginAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Always responds the same way whether or not the email exists — see
    /// UserService.ForgotPasswordAsync. No [Authorize]: this is exactly for people who
    /// can't log in.
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _userService.ForgotPasswordAsync(request);
        return Ok(new { message = "If that email exists, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _userService.ResetPasswordAsync(request);
        return Ok(new { message = "Your password has been reset. You can now log in." });
    }

    /// <summary>
    /// Lets an already-authenticated Admin/MasterAdmin create a new account, including
    /// Admin accounts (never MasterAdmin — see UserService.CreateByAdminAsync).
    /// </summary>
    [HttpPost("admin-create")]
    [Authorize(Roles = "MasterAdmin,Admin,Manager")]
    public async Task<IActionResult> AdminCreate([FromBody] AdminCreateUserRequest request)
    {
        var actorUserId = _currentUserService.UserId!.Value;
        var actorUsername = _currentUserService.Username!;
        var actorRole = _currentUserService.Role!.Value;
        var result = await _userService.CreateByAdminAsync(actorUserId, actorUsername, actorRole, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Only the profile owner or an Admin/MasterAdmin can view the full record.</summary>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var callerId = _currentUserService.UserId!.Value;
        var callerRole = _currentUserService.Role!.Value;
        var result = await _userService.GetByIdAsync(id, callerId, callerRole);
        return Ok(result);
    }

    /// <summary>Only the profile owner or an Admin/MasterAdmin can edit the record.</summary>
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var callerId = _currentUserService.UserId!.Value;
        var callerUsername = _currentUserService.Username!;
        var callerRole = _currentUserService.Role!.Value;
        var result = await _userService.UpdateAsync(id, callerId, callerUsername, callerRole, request);
        return Ok(result);
    }
}
