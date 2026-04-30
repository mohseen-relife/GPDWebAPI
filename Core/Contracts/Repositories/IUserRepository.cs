using Entities;

namespace Contracts.Repositories
{
    /// <summary>
    /// Data access contract for user queries (used for authentication).
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Returns the user matching the given username, or null if not found.
        /// Only returns active users (active = 1).
        /// </summary>
        Task<AppUser?> GetActiveUserByUsernameAsync(string username);
    }
}
