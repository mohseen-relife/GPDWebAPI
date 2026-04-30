using Contracts.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    /// <summary>
    /// User data access — used by AuthManager for login validation.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly GpdDbContext _context;

        public UserRepository(GpdDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the active user with the given username, or null.
        /// active = 1 is enforced — inactive users cannot log in.
        /// </summary>
        public async Task<AppUser?> GetActiveUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Where(u => u.Username == username && u.Active == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
