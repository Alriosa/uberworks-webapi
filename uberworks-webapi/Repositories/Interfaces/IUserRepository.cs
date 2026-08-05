// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es el "contrato" (interface) de acceso a datos para User — define QUÉ
//           operaciones existen (buscar por id, por email, verificar si existe, crear,
//           actualizar) sin decir CÓMO se hacen. UserService.cs depende de esta interface,
//           no de la clase concreta UserRepository — así se puede reemplazar la
//           implementación (ej. para tests) sin tocar la lógica de negocio.
// Entidades relacionadas: User.cs
// Tablas relacionadas: TBL_USERS (indirectamente, a través de UserRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
