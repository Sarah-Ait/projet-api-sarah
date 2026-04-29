using backend.DTOs;
using backend.Services;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<AuthResult> RefreshAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
    }
}