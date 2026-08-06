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
// Entities connected: Professional.cs, User.cs
// Tables related: TBL_PROFESSIONALS, TBL_USERS
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

    public ProfessionalService(IProfessionalRepository professionalRepository, IUserRepository userRepository, IAuditLogService auditLogService)
    {
        _professionalRepository = professionalRepository;
        _userRepository = userRepository;
        _auditLogService = auditLogService;
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

    public async Task<ProfessionalResponse> UpdateAsync(int id, UpdateProfessionalRequest request)
    {
        var professional = await _professionalRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Professional with id {id} was not found.");

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
        CompanyUserId = professional.CompanyUserId
    };
}
