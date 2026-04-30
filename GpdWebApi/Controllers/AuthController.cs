using Contracts.Managers;
using DTO.BinCollections;
using Microsoft.AspNetCore.Mvc;

namespace GpdWebApi.Controllers
{
    /// <summary>
    /// Authentication controller.
    /// POST /api/auth/login  — validates credentials and returns JWT token.
    /// No [Authorize] on this controller — it is intentionally public.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        /// <summary>
        /// Login with username and password from the gpsgateserver.users table.
        /// Returns a JWT Bearer token to use in subsequent requests.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/login
        ///     {
        ///         "username": "tandeef200",
        ///         "password": "yourpassword"
        ///     }
        ///
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authManager.LoginAsync(request);

            if (!response.Success)
                return Unauthorized(new { success = false, message = response.Message });

            return Ok(response);
        }
    }
}
