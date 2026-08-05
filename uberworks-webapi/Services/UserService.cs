// =====================================================================================
// FILE SUMMARY
// What it does: Holds ALL the user business logic: register (rejecting administrator
//               roles), log in (verifying the password and issuing the JWT via
//               IJwtTokenService), find by id, and update basic data. Controllers
//               (UsersController.cs) never talk directly to the database — they always go
//               through here, and this Service never talks directly to SQL — it always goes
//               through IUserRepository.
// Entities connected: User.cs
// Tables related: TBL_USERS (indirectly, via IUserRepository)
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
        // MasterAdmin and Admin are never created through this public endpoint:
        // MasterAdmin is seeded on API startup (Data/Seed/MasterAdminSeeder),
        // and Admin can only be created by another already-authenticated Admin/MasterAdmin
        // (dedicated endpoint pending).
        if (request.Role is UserRole.MasterAdmin or UserRole.Admin)
        {
            throw new ArgumentException("Administrator accounts cannot be registered through this endpoint.");
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new ConflictException($"A user with the email '{request.Email}' already exists.");
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
            throw new InvalidCredentialsException("Invalid email or password.");
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
            ?? throw new NotFoundException($"User with id {id} was not found.");

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"User with id {id} was not found.");

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
