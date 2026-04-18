using backend.Models;

namespace backend.Interfaces
{
    public interface IKanbanColumnService
    {
        Task<List<KanbanColumn>> GetAllKanbanColumnsAsync();
        Task<KanbanColumn?> GetKanbanColumnByIdAsync(int id);
        Task<KanbanColumn> CreateKanbanColumnAsync(CreateKanbanColumnDto createKanbanColumnDto);
    }
}