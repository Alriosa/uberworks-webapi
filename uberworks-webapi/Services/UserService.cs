// =====================================================================================
// FILE SUMMARY
// What it does: Holds ALL the user business logic: register (rejecting administrator
//               roles), log in (verifying the password and issuing the JWT via
//               IJwtTokenService), find by id, and update basic data. GetByIdAsync and
//               UpdateAsync enforce an ownership rule (EnsureSelfOrAdmin): only the profile
//               owner or an Admin/MasterAdmin can view or edit a user's full data (email,
//               phone) — anyone else gets a 403, which is what stops random scraping of the
//               user table by id. Every write action (register, login success/failed,
//               update) is recorded via IAuditLogService: self-actions go to
//               UserActionLog, and an Admin/MasterAdmin editing someone ELSE's account goes
//               to AdminActionLog instead — that split is exactly what UpdateAsync decides
//               based on whether id == callerUserId. Controllers (UsersController.cs) never
//               talk directly to the database — they always go through here, and this
//               Service never talks directly to SQL — it always goes through IUserRepository.
//               CreateByAdminAsync is the counterpart to RegisterAsync for Admin/MasterAdmin
//               callers: it allows creating Admin accounts too (never MasterAdmin) and always
//               logs to AdminActionLog, since the actor is never the target.
// Entities connected: User.cs
// Tables related: TBL_USERS (indirectly, via IUserRepository); TBL_USER_ACTION_LOGS,
//                 TBL_ADMIN_ACTION_LOGS (indirectly, via IAuditLogService)
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
    private readonly IAuditLogService _auditLogService;

    public UserService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _auditLogService = auditLogService;
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

        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            throw new ConflictException($"The username '{request.Username}' is already taken.");
        }

        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = request.Role
        };

        await _userRepository.AddAsync(user);

        await _auditLogService.LogUserActionAsync(
            actorUserId: user.Id,
            actorUsername: user.Username,
            action: "USER_REGISTERED",
            targetEntityType: "User",
            targetEntityId: user.Id,
            details: $"Role={user.Role}, Email={user.Email}");

        return MapToResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            await _auditLogService.LogUserActionAsync(
                actorUserId: user?.Id,
                actorUsername: user?.Username ?? request.Email,
                action: "LOGIN_FAILED",
                targetEntityType: "User",
                targetEntityId: user?.Id,
                details: user is null
                    ? $"No account found for email '{request.Email}'."
                    : "Incorrect password.");

            throw new InvalidCredentialsException("Invalid email or password.");
        }

        await _auditLogService.LogUserActionAsync(
            actorUserId: user.Id,
            actorUsername: user.Username,
            action: "LOGIN_SUCCESS",
            targetEntityType: "User",
            targetEntityId: user.Id);

        var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            User = MapToResponse(user),
            Token = token,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public async Task<UserResponse> CreateByAdminAsync(int actorUserId, string actorUsername, UserRole actorRole, AdminCreateUserRequest request)
    {
        // Only MasterAdmin/Admin can reach this (enforced by [Authorize(Roles=...)] on the
        // controller), but MasterAdmin itself can still never be created here — there must
        // only ever be one, and it only comes from Data/Seed/MasterAdminSeeder.cs.
        if (request.Role == UserRole.MasterAdmin)
        {
            throw new ArgumentException("The MasterAdmin account cannot be created through this endpoint.");
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new ConflictException($"A user with the email '{request.Email}' already exists.");
        }

        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            throw new ConflictException($"The username '{request.Username}' is already taken.");
        }

        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = request.Role
        };

        await _userRepository.AddAsync(user);

        await _auditLogService.LogAdminActionAsync(
            actorUserId: actorUserId,
            actorUsername: actorUsername,
            actorRole: actorRole,
            action: "USER_CREATED_BY_ADMIN",
            targetEntityType: "User",
            targetEntityId: user.Id,
            details: $"Role={user.Role}, Email={user.Email}");

        return MapToResponse(user);
    }

    public async Task<UserResponse> GetByIdAsync(int id, int callerUserId, UserRole callerRole)
    {
        EnsureSelfOrAdmin(id, callerUserId, callerRole);

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"User with id {id} was not found.");

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(int id, int callerUserId, string callerUsername, UserRole callerRole, UpdateUserRequest request)
    {
        EnsureSelfOrAdmin(id, callerUserId, callerRole);

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"User with id {id} was not found.");

        var details = $"Before: [FirstName={user.FirstName}, LastName={user.LastName}, Phone={user.Phone}]. " +
                       $"After: [FirstName={request.FirstName}, LastName={request.LastName}, Phone={request.Phone}].";

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;

        await _userRepository.UpdateAsync(user);

        if (id == callerUserId)
        {
            await _auditLogService.LogUserActionAsync(
                actorUserId: callerUserId,
                actorUsername: callerUsername,
                action: "USER_PROFILE_UPDATED",
                targetEntityType: "User",
                targetEntityId: id,
                details: details);
        }
        else
        {
            await _auditLogService.LogAdminActionAsync(
                actorUserId: callerUserId,
                actorUsername: callerUsername,
                actorRole: callerRole,
                action: "USER_PROFILE_UPDATED_BY_ADMIN",
                targetEntityType: "User",
                targetEntityId: id,
                details: details);
        }

        return MapToResponse(user);
    }

    // Only the profile owner or an Admin/MasterAdmin can view/edit a user's full record.
    // This is the actual fix against scraping: nobody can iterate id=1..N and pull
    // everyone's email/phone anymore.
    private static void EnsureSelfOrAdmin(int targetUserId, int callerUserId, UserRole callerRole)
    {
        var isSelf = targetUserId == callerUserId;
        var isAdmin = callerRole is UserRole.Admin or UserRole.MasterAdmin;

        if (!isSelf && !isAdmin)
        {
            throw new ForbiddenException("You can only view or edit your own profile.");
        }
    }

    private static UserResponse MapToResponse(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone,
        Role = user.Role,
        Status = user.Status,
        RegistrationDate = user.RegistrationDate
    };
}
