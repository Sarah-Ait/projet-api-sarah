using backend.Interfaces;
using backend.Models;


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

        public async Task<User?> CreateUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return null;
            }

            if (!user.Email.Contains("@"))
            {
                return null;
            }

            if (user.Name.Length > 100)
            {
                return null;
            }

            if (user.Email.Length > 150)
            {
                return null;
            }

            return await _userRepository.CreateAsync(user);
        }
    }
}