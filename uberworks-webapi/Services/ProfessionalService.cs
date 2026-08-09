// =====================================================================================
// FILE SUMMARY
// What it does: Business logic to create/query/update a Professional profile. The
//               important rule lives in CreateAsync: it checks that the user exists, that
//               their Role is Professional, and that they don't already have a profile —
//               before inserting the row. The userId always comes from the caller's JWT
//               (ProfessionalsController.cs), never from what the client sends in the body.
//               CreateByCompanyAsync is a different path entirely: a Company account uses
//               it to create a brand-new worker (User + Professional in one call, both from
//               scratch) instead of an existing Professional-role user completing their own
//               profile — see Common/Helpers/PasswordHasher.cs for why it needs to hash a
//               password here too, unlike CreateAsync.
//               GetAcceptedWorkTypesAsync answers "which job categories has this professional
//               actually been hired for" by delegating to
//               IServiceProfessionalRepository.GetAcceptedWorkTypeNamesAsync.
// Entities connected: Professional.cs, User.cs
// Tables related: TBL_PROFESSIONALS, TBL_USERS, TBL_SERVICE_PROFESSIONALS,
//                 TBL_SERVICES, TBL_WORKTYPES (indirectly, via GetAcceptedWorkTypesAsync)
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

public class ProfessionalService : IProfessionalService
{
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IServiceProfessionalRepository _serviceProfessionalRepository;

    // Capped to 3 per the "trabajos que puede realizar" section design — see
    // GetAcceptedWorkTypesAsync below.
    private const int MaxAcceptedWorkTypes = 3;

    public ProfessionalService(
        IProfessionalRepository professionalRepository,
        IUserRepository userRepository,
        IAuditLogService auditLogService,
        IServiceProfessionalRepository serviceProfessionalRepository)
    {
        _professionalRepository = professionalRepository;
        _userRepository = userRepository;
        _auditLogService = auditLogService;
        _serviceProfessionalRepository = serviceProfessionalRepository;
    }

    public async Task<ProfessionalResponse> CreateAsync(int userId, CreateProfessionalRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"User with id {userId} was not found.");

        if (user.Role != UserRole.Professional)
        {
            throw new ConflictException($"User {userId} does not have the PROFESSIONAL role.");
        }

        if (await _professionalRepository.ExistsByUserIdAsync(userId))
        {
            throw new ConflictException($"User {userId} already has a professional profile.");
        }

        var professional = new Professional
        {
            UserId = userId,
            Description = request.Description ?? string.Empty,
            Experience = request.Experience ?? string.Empty,
            Availability = request.Availability ?? string.Empty,
            Location = request.Location ?? string.Empty
        };

        await _professionalRepository.AddAsync(professional);
        professional.User = user;

        return MapToResponse(professional);
    }

    public async Task<ProfessionalResponse> GetByIdAsync(int id)
    {
        var professional = await _professionalRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Professional with id {id} was not found.");

        return MapToResponse(professional);
    }

    public async Task<ProfessionalResponse> GetByUserIdAsync(int userId)
    {
        var professional = await _professionalRepository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException($"User {userId} does not have a professional profile.");

        return MapToResponse(professional);
    }

    public async Task<IReadOnlyList<string>> GetAcceptedWorkTypesAsync(int userId)
    {
        var professional = await _professionalRepository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException($"User {userId} does not have a professional profile.");

        return await _serviceProfessionalRepository.GetAcceptedWorkTypeNamesAsync(professional.Id, MaxAcceptedWorkTypes);
    }

    public async Task<ProfessionalResponse> UpdateAsync(int id, int callerUserId, UserRole callerRole, UpdateProfessionalRequest request)
    {
        var professional = await _professionalRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Professional with id {id} was not found.");

        EnsureSelfOrAdmin(professional.UserId, callerUserId, callerRole);

        professional.Description = request.Description ?? string.Empty;
        professional.Experience = request.Experience ?? string.Empty;
        professional.Availability = request.Availability ?? string.Empty;
        professional.Location = request.Location ?? string.Empty;

        await _professionalRepository.UpdateAsync(professional);

        return MapToResponse(professional);
    }

    public async Task<ProfessionalResponse> CreateByCompanyAsync(int companyUserId, string companyUsername, CompanyCreateWorkerRequest request)
    {
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
            Role = UserRole.Professional
        };

        await _userRepository.AddAsync(user);

        var professional = new Professional
        {
            UserId = user.Id,
            Description = request.Description ?? string.Empty,
            Experience = request.Experience ?? string.Empty,
            Availability = request.Availability ?? string.Empty,
            Location = request.Location ?? string.Empty,
            CompanyUserId = companyUserId
        };

        await _professionalRepository.AddAsync(professional);
        professional.User = user;

        await _auditLogService.LogAdminActionAsync(
            actorUserId: companyUserId,
            actorUsername: companyUsername,
            actorRole: UserRole.Company,
            action: "WORKER_CREATED_BY_COMPANY",
            targetEntityType: "User",
            targetEntityId: user.Id,
            details: $"Email={user.Email}");

        return MapToResponse(professional);
    }

    public async Task<List<ProfessionalResponse>> GetByCompanyUserIdAsync(int companyUserId)
    {
        var professionals = await _professionalRepository.GetByCompanyUserIdAsync(companyUserId);
        return professionals.Select(MapToResponse).ToList();
    }

    public async Task<List<ProfessionalResponse>> GetMyCompanyWorkersAsync(int callerUserId, UserRole callerRole)
    {
        var companyUserId = await ResolveCompanyUserIdAsync(callerUserId, callerRole);
        return await GetByCompanyUserIdAsync(companyUserId);
    }

    public async Task<ProfessionalResponse> LinkExistingWorkerAsync(int callerUserId, UserRole callerRole, string contact)
    {
        var companyUserId = await ResolveCompanyUserIdAsync(callerUserId, callerRole);

        var user = await _userRepository.FindByContactAsync(contact)
            ?? throw new NotFoundException($"No user was found matching '{contact}'.");

        if (user.Role != UserRole.Professional)
        {
            throw new ConflictException($"'{contact}' does not belong to a Professional account.");
        }

        var professional = await _professionalRepository.GetByUserIdAsync(user.Id)
            ?? throw new NotFoundException($"User '{contact}' has no professional profile yet.");

        if (professional.CompanyUserId is int existingCompanyId && existingCompanyId != companyUserId)
        {
            throw new ConflictException("This professional is already linked to a different company.");
        }

        professional.CompanyUserId = companyUserId;
        await _professionalRepository.UpdateAsync(professional);

        await _auditLogService.LogAdminActionAsync(
            actorUserId: callerUserId,
            actorUsername: user.Username,
            actorRole: callerRole,
            action: "WORKER_LINKED_TO_COMPANY",
            targetEntityType: "Professional",
            targetEntityId: professional.Id,
            details: $"CompanyUserId={companyUserId}, WorkerEmail={user.Email}");

        return MapToResponse(professional);
    }

    public async Task UnlinkWorkerAsync(int callerUserId, UserRole callerRole, int professionalId)
    {
        var companyUserId = await ResolveCompanyUserIdAsync(callerUserId, callerRole);

        var professional = await _professionalRepository.GetByIdAsync(professionalId)
            ?? throw new NotFoundException($"Professional with id {professionalId} was not found.");

        if (professional.CompanyUserId != companyUserId)
        {
            throw new ForbiddenException("You can only remove workers linked to your own company.");
        }

        professional.CompanyUserId = null;
        await _professionalRepository.UpdateAsync(professional);

        await _auditLogService.LogAdminActionAsync(
            actorUserId: callerUserId,
            actorUsername: professional.User.Username,
            actorRole: callerRole,
            action: "WORKER_UNLINKED_FROM_COMPANY",
            targetEntityType: "Professional",
            targetEntityId: professional.Id,
            details: $"CompanyUserId={companyUserId}");
    }

    // Same resolution rule as UserService.ResolveCompanyUserIdAsync (duplicated rather than
    // shared — it's a 5-line lookup, not worth a new cross-service abstraction): a Company
    // acts on its own behalf; a Manager acts on behalf of whichever company it belongs to.
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

    // photoUrl is already the saved, relative path by the time it gets here —
    // ProfessionalsController.UploadPhoto is what actually writes the file to disk.
    public async Task<ProfessionalResponse> UpdatePhotoAsync(int id, int callerUserId, UserRole callerRole, string photoUrl)
    {
        var professional = await _professionalRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Professional with id {id} was not found.");

        EnsureSelfOrAdmin(professional.UserId, callerUserId, callerRole);

        professional.PhotoUrl = photoUrl;
        await _professionalRepository.UpdateAsync(professional);

        return MapToResponse(professional);
    }

    // Only the profile owner or an Admin/MasterAdmin can edit it — same rule and same
    // reasoning as UserService.EnsureSelfOrAdmin: this is what stops anyone from editing
    // another professional's profile just by guessing their numeric id in the URL.
    private static void EnsureSelfOrAdmin(int targetUserId, int callerUserId, UserRole callerRole)
    {
        var isSelf = targetUserId == callerUserId;
        var isAdmin = callerRole is UserRole.Admin or UserRole.MasterAdmin;

        if (!isSelf && !isAdmin)
        {
            throw new ForbiddenException("You can only edit your own professional profile.");
        }
    }

    private static ProfessionalResponse MapToResponse(Professional professional) => new()
    {
        Id = professional.Id,
        UserId = professional.UserId,
        Username = professional.User.Username,
        FirstName = professional.User.FirstName,
        LastName = professional.User.LastName,
        Description = professional.Description,
        Experience = professional.Experience,
        Availability = professional.Availability,
        Location = professional.Location,
        AverageRating = professional.AverageRating,
        CompanyUserId = professional.CompanyUserId,
        PhotoUrl = professional.PhotoUrl
    };
}
