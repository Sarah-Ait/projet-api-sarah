using backend.Interfaces;
using backend.Models;
using backend.DTOs;

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

        public async Task<KanbanColumn> CreateKanbanColumnAsync(CreateKanbanColumnDto createKanbanColumnDto)
        {
            var user = await _userRepository.GetByIdAsync(createKanbanColumnDto.UserId);

            if (user == null)
            {
                return null;
            }

            var kanbanColumn = new KanbanColumn
            {
                Name = createKanbanColumnDto.Name,
                Order = createKanbanColumnDto.Order,
                UserId = createKanbanColumnDto.UserId
            };

            return await _kanbanColumnRepository.CreateAsync(kanbanColumn);
        }
    }
}