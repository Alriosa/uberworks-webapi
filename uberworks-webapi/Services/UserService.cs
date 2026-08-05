// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contiene TODA la lógica de negocio de usuarios: registrar (rechazando roles
//           de administrador), hacer login (verificando password y emitiendo el JWT vía
//           IJwtTokenService), buscar por id, y actualizar datos básicos. Los Controllers
//           (UsersController.cs) nunca hablan directo con la base de datos — siempre pasan
//           por aquí, y este Service nunca habla directo con SQL — siempre pasa por
//           IUserRepository. Explicación completa de JWT al final de la respuesta del chat.
// Entidades relacionadas: User.cs
// Tablas relacionadas: TBL_USERS (indirectamente, vía IUserRepository)
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Common.Helpers;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public UserService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request)
    {
        // MasterAdmin y Admin nunca se crean por este endpoint público:
        // MasterAdmin se siembra al arrancar la API (Data/Seed/MasterAdminSeeder),
        // y Admin solo lo puede crear otro Admin/MasterAdmin ya autenticado (pendiente de endpoint dedicado).
        if (request.Role is UserRole.MasterAdmin or UserRole.Admin)
        {
            throw new ArgumentException("No es posible registrar cuentas de administrador desde este endpoint.");
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new ConflictException($"Ya existe un usuario registrado con el email '{request.Email}'.");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = request.Role
        };

        await _userRepository.AddAsync(user);

        return MapToResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException("Email o contraseña incorrectos.");
        }

        var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            User = MapToResponse(user),
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el usuario con id {id}.");

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el usuario con id {id}.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;

        await _userRepository.UpdateAsync(user);

        return MapToResponse(user);
    }

    private static UserResponse MapToResponse(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone,
        Role = user.Role,
        Status = user.Status,
        RegistrationDate = user.RegistrationDate
    };
}
