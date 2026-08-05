// =====================================================================================
// FILE SUMMARY
// What it does: Business logic to create/query/update a Professional profile. The
//               important rule lives in CreateAsync: it checks that the user exists, that
//               their Role is Professional, and that they don't already have a profile —
//               before inserting the row. The userId always comes from the caller's JWT
//               (ProfessionalsController.cs), never from what the client sends in the body.
// Entities connected: Professional.cs, User.cs
// Tables related: TBL_PROFESSIONALS, TBL_USERS
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Exceptions;
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

    public ProfessionalService(IProfessionalRepository professionalRepository, IUserRepository userRepository)
    {
        _professionalRepository = professionalRepository;
        _userRepository = userRepository;
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
            Description = request.Description,
            Experience = request.Experience,
            Availability = request.Availability,
            Location = request.Location
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

        professional.Description = request.Description;
        professional.Experience = request.Experience;
        professional.Availability = request.Availability;
        professional.Location = request.Location;

        await _professionalRepository.UpdateAsync(professional);

        return MapToResponse(professional);
    }

    private static ProfessionalResponse MapToResponse(Professional professional) => new()
    {
        Id = professional.Id,
        UserId = professional.UserId,
        FirstName = professional.User.FirstName,
        LastName = professional.User.LastName,
        Email = professional.User.Email,
        Description = professional.Description,
        Experience = professional.Experience,
        Availability = professional.Availability,
        Location = professional.Location,
        AverageRating = professional.AverageRating
    };
}
