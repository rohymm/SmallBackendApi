using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmallBackendApi.Dtos;
using SmallBackendApi.Models;
using SmallBackendApi.Services;

namespace SmallBackendApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>Login with email and password, returns a JWT access token.</summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // Same message for "no such user" and "wrong password": do not reveal which one failed
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                _logger.LogWarning("Failed API login attempt");
                return Unauthorized("Invalid email or password.");
            }

            var (token, expiresAt) = _tokenService.CreateToken(user);

            return Ok(new TokenResponseDto { Token = token, ExpiresAt = expiresAt });
        }
    }
}