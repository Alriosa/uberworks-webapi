// =====================================================================================
// FILE SUMMARY
// What it does: This is the data-access "contract" (interface) for User — defines WHAT
//               operations exist (find by id, by email, check existence, create, update)
//               without saying HOW they're done. UserService.cs depends on this interface,
//               not on the concrete UserRepository class — so the implementation can be
//               swapped (e.g. for tests) without touching business logic.
// Entities connected: User.cs
// Tables related: TBL_USERS (indirectly, through UserRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);

    /// <summary>Matches on email, username, OR phone — see ProfessionalService's "link existing worker" flow.</summary>
    Task<User?> FindByContactAsync(string contact);

    Task<IReadOnlyList<User>> GetAllAsync();
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
