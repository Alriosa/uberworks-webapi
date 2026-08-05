// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Expone los endpoints HTTP de registro, login, consulta y actualización de
//           usuarios. El Controller solo recibe/valida forma y delega toda la lógica real
//           a IUserService — nunca decide reglas de negocio ni toca la base de datos.
// Entidades relacionadas: User.cs (indirectamente, vía IUserService)
// Tablas relacionadas: TBL_USERS (indirectamente, a través de todas las capas)
// =====================================================================================
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateAsync(id, request);
        return Ok(result);
    }
}
