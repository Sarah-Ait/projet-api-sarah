using backend.Models;
using backend.DTOs;

namespace backend.Interfaces
{
    public interface IKanbanColumnService
    {
        Task<List<KanbanColumn>> GetAllKanbanColumnsAsync();
        Task<KanbanColumn?> GetKanbanColumnByIdAsync(int id);
        Task<KanbanColumn?> CreateKanbanColumnAsync(CreateKanbanColumnDto createKanbanColumnDto);
        Task<bool> DeleteKanbanColumnAsync(int id);
    }
}