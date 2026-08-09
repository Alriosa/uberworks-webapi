// =====================================================================================
// FILE SUMMARY
// What it does: Holds ALL the user business logic: register (always as Client — see
//               UserRole.cs for the full account-creation pyramid), log in (verifying the
//               password and issuing the JWT via
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
//               logs to AdminActionLog, since the actor is never the target. ExternalLoginAsync
//               backs Google AND Facebook sign-in: logs an existing user in (linking their
//               FacebookId the first time they use Facebook), or auto-creates a new
//               Role=Client account if the (provider-verified) email is new — new accounts
//               get IsPasswordSet=false, which AuthResponse.RequiresPasswordSetup surfaces to
//               the WebApp so it can prompt for a real password. SetPasswordAsync is how that
//               prompt actually sets one, for an already-authenticated caller.
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
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    // How long a "forgot password" link stays valid before it's treated as expired
    // (same as if it never existed — see ResetPasswordAsync).
    private static readonly TimeSpan PasswordResetTokenLifetime = TimeSpan.FromHours(1);

    public UserService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IAuditLogService auditLogService,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _auditLogService = auditLogService;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request)
    {
        // This is the ONLY role this endpoint can ever create — see the account-creation
        // pyramid documented on UserRole.cs. Professional/Manager/Company/Admin accounts
        // can only come from CreateByAdminAsync (an already-authenticated Manager/Admin/
        // MasterAdmin); MasterAdmin only from Data/Seed/MasterAdminSeeder.cs.
        const UserRole role = UserRole.Client;

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
            Role = role
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

        // Suspended/Deleted accounts must not be able to log back in — otherwise an Admin
        // "deleting" or suspending someone from the dashboard would do nothing real.
        // Penalized is deliberately NOT blocked here: a penalty is a mark against the
        // account, not a full lockout.
        if (user.Status is UserStatus.Suspended or UserStatus.Deleted)
        {
            await _auditLogService.LogUserActionAsync(
                actorUserId: user.Id,
                actorUsername: user.Username,
                action: "LOGIN_BLOCKED",
                targetEntityType: "User",
                targetEntityId: user.Id,
                details: $"Status={user.Status}");

            throw new InvalidCredentialsException("This account is no longer active. Contact support if you believe this is a mistake.");
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
            ExpiresAtUtc = expiresAtUtc,
            RequiresPasswordSetup = !user.IsPasswordSet
        };
    }

    public async Task<AuthResponse> ExternalLoginAsync(ExternalLoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            // First time this email is seen from Google/Facebook: auto-create a Client
            // account. Never MasterAdmin/Admin — same rule as RegisterAsync. The password is
            // a random value nobody knows (IsPasswordSet=false is what actually matters:
            // that's what tells the WebApp to prompt for a real password after sign-in).
            var randomPassword = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

            user = new User
            {
                Username = await GenerateUniqueUsernameAsync(request.Email),
                FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? request.Provider.ToString() : request.FirstName,
                LastName = string.IsNullOrWhiteSpace(request.LastName) ? "User" : request.LastName,
                Email = request.Email,
                PasswordHash = PasswordHasher.Hash(randomPassword),
                Role = UserRole.Client,
                FacebookId = request.Provider == AuthProvider.Facebook ? request.ProviderUserId : null,
                IsPasswordSet = false
            };

            await _userRepository.AddAsync(user);

            await _auditLogService.LogUserActionAsync(
                actorUserId: user.Id,
                actorUsername: user.Username,
                action: "USER_REGISTERED",
                targetEntityType: "User",
                targetEntityId: user.Id,
                details: $"Role={user.Role}, Email={user.Email}, Provider={request.Provider}");
        }
        else if (request.Provider == AuthProvider.Facebook && user.FacebookId is null && !string.IsNullOrEmpty(request.ProviderUserId))
        {
            // Existing account (registered normally, or via Google) signing in with Facebook
            // for the first time using the same email — link the Facebook account to it so
            // future Facebook logins are recognized as this same person.
            user.FacebookId = request.ProviderUserId;
            await _userRepository.UpdateAsync(user);
        }

        await _auditLogService.LogUserActionAsync(
            actorUserId: user.Id,
            actorUsername: user.Username,
            action: "LOGIN_SUCCESS",
            targetEntityType: "User",
            targetEntityId: user.Id,
            details: $"Provider={request.Provider}");

        var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            User = MapToResponse(user),
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            RequiresPasswordSetup = !user.IsPasswordSet
        };
    }

    // Lets a Google/Facebook-created account (User.IsPasswordSet == false) set a real
    // password for the first time. Called from an [Authorize]'d endpoint, so userId comes
    // from the caller's own JWT — nobody can set someone else's password through this.
    public async Task SetPasswordAsync(int userId, SetPasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"User with id {userId} was not found.");

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        user.IsPasswordSet = true;
        await _userRepository.UpdateAsync(user);

        await _auditLogService.LogUserActionAsync(
            actorUserId: user.Id,
            actorUsername: user.Username,
            action: "PASSWORD_SET_COMPLETED",
            targetEntityType: "User",
            targetEntityId: user.Id);
    }

    // Builds a username from the email's local part (before the @), appending a random
    // 4-digit suffix if that username is already taken — Google never provides a username.
    private async Task<string> GenerateUniqueUsernameAsync(string email)
    {
        var baseUsername = email.Split('@')[0];

        if (!await _userRepository.ExistsByUsernameAsync(baseUsername))
        {
            return baseUsername;
        }

        string candidate;
        do
        {
            candidate = $"{baseUsername}{Random.Shared.Next(1000, 9999)}";
        }
        while (await _userRepository.ExistsByUsernameAsync(candidate));

        return candidate;
    }

    // The account-creation pyramid (see UserRole.cs): each role can create everything
    // below it, never itself or above. MasterAdmin is never in any list — there must only
    // ever be one, and it only comes from Data/Seed/MasterAdminSeeder.cs. [Authorize(Roles
    // = "MasterAdmin,Admin,Manager")] on the controller already filters out Client/
    // Professional/Company callers entirely; this dictionary is what stops, say, a Manager
    // from creating another Manager or an Admin.
    private static readonly Dictionary<UserRole, UserRole[]> CreatableRolesByActor = new()
    {
        [UserRole.MasterAdmin] = [UserRole.Admin, UserRole.Manager, UserRole.Company, UserRole.Support, UserRole.Professional, UserRole.Client],
        [UserRole.Admin] = [UserRole.Manager, UserRole.Company, UserRole.Support, UserRole.Professional, UserRole.Client],
        [UserRole.Manager] = [UserRole.Company, UserRole.Professional, UserRole.Client]
    };

    public async Task<UserResponse> CreateByAdminAsync(int actorUserId, string actorUsername, UserRole actorRole, AdminCreateUserRequest request)
    {
        if (!CreatableRolesByActor.TryGetValue(actorRole, out var creatableRoles) || !creatableRoles.Contains(request.Role))
        {
            throw new ForbiddenException($"A {actorRole} cannot create a {request.Role} account.");
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

    // "No existe manager sin su empresa" — the new Manager's ManagedByCompanyUserId is always
    // resolved server-side, never taken from the request: a Company creates one linked to
    // itself; an existing Manager creates one linked to the SAME company it already belongs
    // to (so a chain of Managers can onboard each other without ever detaching from the
    // company that originally brought them in).
    public async Task<UserResponse> CreateManagerAsync(int callerUserId, UserRole callerRole, CompanyCreateManagerRequest request)
    {
        var companyUserId = await ResolveCompanyUserIdAsync(callerUserId, callerRole);

        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new ConflictException($"A user with the email '{request.Email}' already exists.");
        }

        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            throw new ConflictException($"The username '{request.Username}' is already taken.");
        }

        var manager = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = UserRole.Manager,
            ManagedByCompanyUserId = companyUserId
        };

        await _userRepository.AddAsync(manager);

        await _auditLogService.LogAdminActionAsync(
            actorUserId: callerUserId,
            actorUsername: request.Username,
            actorRole: callerRole,
            action: "MANAGER_CREATED_BY_COMPANY",
            targetEntityType: "User",
            targetEntityId: manager.Id,
            details: $"CompanyUserId={companyUserId}, Email={manager.Email}");

        return MapToResponse(manager);
    }

    public async Task<UserResponse> GetMyCompanyAsync(int managerUserId)
    {
        var manager = await _userRepository.GetByIdAsync(managerUserId)
            ?? throw new NotFoundException($"User with id {managerUserId} was not found.");

        var companyUserId = manager.ManagedByCompanyUserId
            ?? throw new ForbiddenException("This Manager account has no company linked to it.");

        var company = await _userRepository.GetByIdAsync(companyUserId)
            ?? throw new NotFoundException($"Company with id {companyUserId} was not found.");

        return MapToResponse(company);
    }

    // A Company is always linked to itself; a Manager is linked to whichever company it
    // already belongs to. Throws if somehow a Manager exists with no company — should never
    // happen (CreateManagerAsync always sets it), but this is what stops a null from silently
    // orphaning a newly created Manager instead.
    private async Task<int> ResolveCompanyUserIdAsync(int callerUserId, UserRole callerRole)
    {
        if (callerRole == UserRole.Company)
        {
            return callerUserId;
        }

        var caller = await _userRepository.GetByIdAsync(callerUserId)
            ?? throw new NotFoundException($"User with id {callerUserId} was not found.");

        return caller.ManagedByCompanyUserId
            ?? throw new ForbiddenException("This Manager account has no company linked to it.");
    }

    public async Task<UserResponse> GetByIdAsync(int id, int callerUserId, UserRole callerRole)
    {
        EnsureSelfOrAdmin(id, callerUserId, callerRole);

        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"User with id {id} was not found.");

        return MapToResponse(user);
    }

    // No per-row ownership check here on purpose — [Authorize(Roles = "MasterAdmin,Admin,Manager")]
    // on the controller already restricts who can call this at all; every field returned
    // (still excluding PasswordHash) is meant to be visible to that audience.
    public async Task<IReadOnlyList<AdminUserListItemResponse>> GetAllForAdminAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new AdminUserListItemResponse
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status,
            RegistrationDate = user.RegistrationDate,
            FacebookId = user.FacebookId,
            IsPasswordSet = user.IsPasswordSet
        }).ToList();
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

    // Soft-delete: sets Status=Deleted instead of a real SQL DELETE — see UserStatus.cs for
    // why (foreign keys from Professional/Service/Review/Chat/Penalty/PasswordResetToken all
    // point at TBL_USERS, so a hard delete would either fail outright or silently erase real
    // history). LoginAsync already refuses Deleted accounts, so this is a genuine lockout,
    // not just a cosmetic flag.
    public async Task DeleteAsync(int id, int callerUserId, string callerUsername, UserRole callerRole)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"User with id {id} was not found.");

        if (user.Role == UserRole.MasterAdmin)
        {
            throw new ForbiddenException("The MasterAdmin account cannot be deleted.");
        }

        if (id == callerUserId)
        {
            throw new ForbiddenException("You cannot delete your own account.");
        }

        user.Status = UserStatus.Deleted;
        await _userRepository.UpdateAsync(user);

        await _auditLogService.LogAdminActionAsync(
            actorUserId: callerUserId,
            actorUsername: callerUsername,
            actorRole: callerRole,
            action: "USER_DELETED_BY_ADMIN",
            targetEntityType: "User",
            targetEntityId: id,
            details: $"Role={user.Role}, Email={user.Email}");
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

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // Deliberately silent if the email doesn't exist: returning normally either way is
        // what stops this endpoint from being usable to check which emails are registered.
        if (user is null)
        {
            return;
        }

        var rawToken = SecureTokenHelper.GenerateToken();

        await _passwordResetTokenRepository.AddAsync(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = SecureTokenHelper.Hash(rawToken),
            ExpiresAt = DateTime.UtcNow.Add(PasswordResetTokenLifetime)
        });

        var webAppBaseUrl = _configuration["WebApp:BaseUrl"]
            ?? throw new InvalidOperationException("WebApp:BaseUrl is not configured in appsettings.");
        var resetLink = $"{webAppBaseUrl}/Account/ResetPassword?token={Uri.EscapeDataString(rawToken)}";

        await _emailSender.SendAsync(
            user.Email,
            "Reset your Uberworks password",
            $"""
             <p>Hi {user.FirstName},</p>
             <p>Someone (hopefully you) requested to reset your Uberworks password.</p>
             <p><a href="{resetLink}">Click here to choose a new password</a>. This link expires in 1 hour and can only be used once.</p>
             <p>If you didn't request this, you can safely ignore this email — your password won't change.</p>
             """);

        await _auditLogService.LogUserActionAsync(
            actorUserId: user.Id,
            actorUsername: user.Username,
            action: "PASSWORD_RESET_REQUESTED",
            targetEntityType: "User",
            targetEntityId: user.Id);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var tokenHash = SecureTokenHelper.Hash(request.Token);
        var token = await _passwordResetTokenRepository.GetValidByTokenHashAsync(tokenHash)
            ?? throw new InvalidCredentialsException("This password reset link is invalid or has expired.");

        token.User.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        await _userRepository.UpdateAsync(token.User);

        token.Used = true;
        await _passwordResetTokenRepository.UpdateAsync(token);

        await _auditLogService.LogUserActionAsync(
            actorUserId: token.User.Id,
            actorUsername: token.User.Username,
            action: "PASSWORD_RESET_COMPLETED",
            targetEntityType: "User",
            targetEntityId: token.User.Id);
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
