using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public class KanbanColumnService : IKanbanColumnService
    {
        private readonly IKanbanColumnRepository _kanbanColumnRepository;
        private readonly IUserRepository _userRepository;

        public KanbanColumnService(
            IKanbanColumnRepository kanbanColumnRepository,
            IUserRepository userRepository)
        {
            _kanbanColumnRepository = kanbanColumnRepository;
            _userRepository = userRepository;
        }

        public async Task<List<KanbanColumn>> GetAllKanbanColumnsAsync()
        {
            return await _kanbanColumnRepository.GetAllAsync();
        }

        public async Task<KanbanColumn?> GetKanbanColumnByIdAsync(int id)
        {
            return await _kanbanColumnRepository.GetByIdAsync(id);
        }

        public async Task<KanbanColumn?> CreateKanbanColumnAsync(KanbanColumn kanbanColumn)
        {
            if (string.IsNullOrWhiteSpace(kanbanColumn.Name))
            {
                return null;
            }

            if (kanbanColumn.Name.Length > 100)
            {
                return null;
            }

            if (kanbanColumn.Order < 0)
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(kanbanColumn.UserId);

            if (user == null)
            {
                return null;
            }

            return await _kanbanColumnRepository.CreateAsync(kanbanColumn);
        }
    }
}