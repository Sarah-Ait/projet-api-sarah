using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using backend.Constants;
namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto loginRequestDto)
        {
            var authResponse = await _authService.LoginAsync(loginRequestDto);

            if (authResponse == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            return Ok(authResponse);
        }
    }
}