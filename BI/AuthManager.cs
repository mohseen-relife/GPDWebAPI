using Contracts.Managers;
using Contracts.Repositories;
using DTO.BinCollections;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BI
{
    /// <summary>
    /// Handles login authentication against the gpsgateserver users table.
    ///
    /// Password scheme detection:
    ///   1. If users.password is set (MD5 char-32) → verify MD5(input) == stored
    ///   2. Otherwise fall back to password_hash + password_salt (Base64 HMAC-SHA1)
    ///
    /// On success, issues a signed HS256 JWT with username + userId claims.
    /// </summary>
    public class AuthManager : IAuthManager
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration  _config;

        public AuthManager(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config   = config;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var response = new LoginResponse();

            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                response.Success = false;
                response.Message = "Username and password are required.";
                return response;
            }

            // 1. Fetch user (active = 1 enforced in repo)
            var user = await _userRepo.GetActiveUserByUsernameAsync(request.Username.Trim());
            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid username or password.";
                return response;
            }

            // 2. Verify password
            bool passwordValid = VerifyPassword(request.Password, user.Password,
                                                user.PasswordHash, user.PasswordSalt);
            if (!passwordValid)
            {
                response.Success = false;
                response.Message = "Invalid username or password.";
                return response;
            }

            // 3. Issue JWT
            var (token, expires) = GenerateJwt(user.UserId, user.Username);

            response.Success  = true;
            response.Token    = token;
            response.Expires  = expires;
            response.Username = user.Username;
            response.Message  = "Login successful.";
            return response;
        }

        // ── Password verification ─────────────────────────────────────────────

        private static bool VerifyPassword(
            string plainPassword,
            string? storedMd5,
            string? storedHash,
            string? storedSalt)
        {
            // Scheme 1 — legacy MD5 (most rows in your DB)
            if (!string.IsNullOrEmpty(storedMd5))
            {
                var md5Input = ComputeMd5(plainPassword);
                if (string.Equals(md5Input, storedMd5, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // Scheme 2 — salted HMAC-SHA1 (newer rows)
            if (!string.IsNullOrEmpty(storedHash) && !string.IsNullOrEmpty(storedSalt))
            {
                try
                {
                    var saltBytes  = Convert.FromBase64String(storedSalt);
                    var keyBytes   = Encoding.UTF8.GetBytes(plainPassword).Concat(saltBytes).ToArray();
                    using var hmac = new HMACSHA1(keyBytes);
                    var computed   = Convert.ToBase64String(hmac.ComputeHash(saltBytes));
                    if (string.Equals(computed, storedHash, StringComparison.Ordinal))
                        return true;
                }
                catch
                {
                    // Malformed salt/hash — fall through to failure
                }
            }

            return false;
        }

        private static string ComputeMd5(string input)
        {
            var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        // ── JWT generation ────────────────────────────────────────────────────

        private (string token, DateTime expires) GenerateJwt(int userId, string username)
        {
            var jwtSection  = _config.GetSection("JwtSettings");
            var secretKey   = jwtSection["SecretKey"]   ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
            var issuer      = jwtSection["Issuer"]      ?? "GpdWebApi";
            var audience    = jwtSection["Audience"]    ?? "GpdWebApiClients";
            var expiryHours = int.TryParse(jwtSection["ExpiryHours"], out var h) ? h : 8;

            var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(expiryHours);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,  username),
                new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
                new Claim("userId",                     userId.ToString()),
                new Claim(ClaimTypes.Name,              username)
            };

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims:             claims,
                notBefore:          DateTime.UtcNow,
                expires:            expires,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }
    }
}
