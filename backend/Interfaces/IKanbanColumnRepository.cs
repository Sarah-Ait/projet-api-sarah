using backend.Models;

namespace backend.Interfaces
{
    public interface IKanbanColumnRepository
    {
        Task<List<KanbanColumn>> GetAllAsync();
        Task<KanbanColumn?> GetByIdAsync(int id);
        Task<KanbanColumn> CreateAsync(KanbanColumn kanbanColumn);
    }
}