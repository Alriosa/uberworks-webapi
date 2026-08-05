// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Lógica de negocio para crear/consultar/actualizar el perfil de Professional.
//           La regla importante está en CreateAsync: valida que el usuario exista, que su
//           Role sea Professional, y que no tenga ya un perfil creado — antes de dejar
//           insertar la fila. El userId siempre llega desde el JWT del que llama
//           (ProfessionalsController.cs), nunca desde lo que mande el cliente en el body.
// Entidades relacionadas: Professional.cs, User.cs
// Tablas relacionadas: TBL_PROFESSIONALS, TBL_USERS
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
            ?? throw new NotFoundException($"No se encontró el usuario con id {userId}.");

        if (user.Role != UserRole.Professional)
        {
            throw new ConflictException($"El usuario {userId} no tiene el rol PROFESSIONAL.");
        }

        if (await _professionalRepository.ExistsByUserIdAsync(userId))
        {
            throw new ConflictException($"El usuario {userId} ya tiene un perfil de profesional.");
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
            ?? throw new NotFoundException($"No se encontró el profesional con id {id}.");

        return MapToResponse(professional);
    }

    public async Task<ProfessionalResponse> GetByUserIdAsync(int userId)
    {
        var professional = await _professionalRepository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException($"El usuario {userId} no tiene un perfil de profesional.");

        return MapToResponse(professional);
    }

    public async Task<ProfessionalResponse> UpdateAsync(int id, UpdateProfessionalRequest request)
    {
        var professional = await _professionalRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el profesional con id {id}.");

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
