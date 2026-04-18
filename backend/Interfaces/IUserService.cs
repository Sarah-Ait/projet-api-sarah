using backend.Models;
using backend.DTOs;

namespace backend.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> CreateUserAsync(CreateUserDto createUserDto);

        Task<bool> DeleteUserAsync(int id);
    }
}