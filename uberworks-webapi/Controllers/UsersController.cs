// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the HTTP endpoints for registration, login, lookup, and update of
//               users. The Controller only receives/validates shape and delegates all real
//               logic to IUserService — it never decides business rules or touches the
//               database. GetById/Update require [Authorize] and pass the caller's identity
//               down to the Service, which enforces that only the profile owner or an
//               Admin/MasterAdmin can view/edit it (see UserService.EnsureSelfOrAdmin) —
//               this is what stops anyone from scraping every user's email/phone by id.
// Entities connected: User.cs (indirectly, via IUserService)
// Tables related: TBL_USERS (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
