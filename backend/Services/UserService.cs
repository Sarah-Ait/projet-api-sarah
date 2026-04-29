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
        private readonly IKanbanColumnRepository _kanbanColumnRepository;
        private readonly PasswordHasher<User> _passwordHasher;

       public UserService(
            IUserRepository userRepository,
            IKanbanColumnRepository kanbanColumnRepository)
        {
            _userRepository = userRepository;
            _kanbanColumnRepository = kanbanColumnRepository;
            _passwordHasher = new PasswordHasher<User>();
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

            user.PasswordHash = _passwordHasher.HashPassword(user, createUserDto.Password);

            var createdUser = await _userRepository.CreateAsync(user);

            var defaultColumns = new List<KanbanColumn>
            {
                new KanbanColumn
                {
                    Name = "À faire",
                    Order = 1,
                    UserId = createdUser.Id
                },
                new KanbanColumn
                {
                    Name = "En cours",
                    Order = 2,
                    UserId = createdUser.Id
                },
                new KanbanColumn
                {
                    Name = "Terminées",
                    Order = 3,
                    UserId = createdUser.Id
                }
            };

            foreach (var column in defaultColumns)
            {
                await _kanbanColumnRepository.CreateAsync(column);
            }

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