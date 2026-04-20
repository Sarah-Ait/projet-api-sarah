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

        private UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserResponseDto).ToList();
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            
            if (user == null)
                return null;

            return MapToUserResponseDto(user);
        }

         public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                PasswordHash = createUserDto.PasswordHash,
                Role = "Standard"
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return MapToUserResponseDto(createdUser);
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