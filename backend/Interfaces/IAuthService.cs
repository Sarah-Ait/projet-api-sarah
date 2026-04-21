using backend.DTOs;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
    }
}