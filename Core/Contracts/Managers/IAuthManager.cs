using DTO.BinCollections;

namespace Contracts.Managers
{
    /// <summary>
    /// Business logic contract for authentication.
    /// Implementation lives in BI.AuthManager.
    /// </summary>
    public interface IAuthManager
    {
        /// <summary>
        /// Validates credentials against the users table and returns a JWT token.
        /// </summary>
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
