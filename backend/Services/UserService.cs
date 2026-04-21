using backend.Interfaces;
using backend.Models;
using backend.DTOs;
using backend.Exceptions;
using Microsoft.AspNetCore.Identity; // permet de hasher les mots de passe

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher; // outil de hash

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>(); // crée l'outil de hash
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

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            
            if (user == null)
                throw new NotFoundException($"User with ID {id} not found");

            return MapToUserResponseDto(user);
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Role = "Standard"
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, createUserDto.Password); // stocke un hash et pas le mot de passe brut
            var createdUser = await _userRepository.CreateAsync(user);
            return MapToUserResponseDto(createdUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);

            if (existingUser == null)
                throw new NotFoundException($"User with ID {id} not found");

            await _userRepository.DeleteAsync(id);
            return true;
        }
    }
}