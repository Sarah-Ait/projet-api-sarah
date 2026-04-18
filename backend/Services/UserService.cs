using backend.Interfaces;
using backend.Models;
using backend.DTOs;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

         public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                PasswordHash = createUserDto.PasswordHash,
                Role = "Standard"
            };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);

            if (existingUser == null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(id);
            return true;
        }
    }
}