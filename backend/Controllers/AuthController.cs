using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const string RefreshTokenCookieName = "refreshToken";
        private const string RefreshTokenCookiePath = "/api/auth";

        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _environment;

        public AuthController(IAuthService authService, IWebHostEnvironment environment)
        {
            _authService = authService;
            _environment = environment;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto loginRequestDto)
        {
            var authResult = await _authService.LoginAsync(loginRequestDto);

            if (authResult == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            SetRefreshTokenCookie(authResult.RefreshToken, authResult.RefreshTokenExpiresAt, authResult.IsPersistent);

            return Ok(MapToAuthResponseDto(authResult));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized(new { message = "Refresh token manquant." });

            var authResult = await _authService.RefreshAsync(refreshToken);

            SetRefreshTokenCookie(authResult.RefreshToken, authResult.RefreshTokenExpiresAt, authResult.IsPersistent);

            return Ok(MapToAuthResponseDto(authResult));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (!string.IsNullOrWhiteSpace(refreshToken))
                await _authService.LogoutAsync(refreshToken);

            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                Path = RefreshTokenCookiePath
            });

            return NoContent();
        }

        private static AuthResponseDto MapToAuthResponseDto(Services.AuthResult authResult)
        {
            return new AuthResponseDto
            {
                AccessToken = authResult.AccessToken,
                ExpiresAt = authResult.AccessTokenExpiresAt,
                Email = authResult.Email,
                Role = authResult.Role
            };
        }

        private void SetRefreshTokenCookie(string token, DateTime expiresAt, bool isPersistent)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !_environment.IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Path = RefreshTokenCookiePath
            };

            if (isPersistent)
                cookieOptions.Expires = expiresAt;

            Response.Cookies.Append(RefreshTokenCookieName, token, cookieOptions);
        }
    }
}
