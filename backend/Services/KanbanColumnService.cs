using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public class KanbanColumnService : IKanbanColumnService
    {
        private readonly IKanbanColumnRepository _kanbanColumnRepository;

        public KanbanColumnService(IKanbanColumnRepository kanbanColumnRepository)
        {
            _kanbanColumnRepository = kanbanColumnRepository;
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

            return await _kanbanColumnRepository.CreateAsync(kanbanColumn);
        }
    }
}