using backend.Models;

namespace backend.Interfaces
{
    public interface IKanbanColumnRepository
    {
        Task<List<KanbanColumn>> GetAllAsync();
        Task<KanbanColumn?> GetByIdAsync(int id);
        Task<List<KanbanColumn>> GetByIdsAsync(IEnumerable<int> ids);
        Task<KanbanColumn> CreateAsync(KanbanColumn kanbanColumn);
        Task<KanbanColumn> UpdateAsync(KanbanColumn kanbanColumn);
        Task UpdateRangeAsync(IEnumerable<KanbanColumn> kanbanColumns);
        Task DeleteAsync(int id);
    }
}