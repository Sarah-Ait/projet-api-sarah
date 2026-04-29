using backend.DTOs;

namespace backend.Interfaces
{
    public interface IKanbanColumnService
    {
        Task<List<KanbanColumnResponseDto>> GetAllKanbanColumnsAsync();
        Task<KanbanColumnResponseDto> GetKanbanColumnByIdAsync(int id);
        Task<KanbanColumnResponseDto> CreateKanbanColumnAsync(CreateKanbanColumnDto createKanbanColumnDto);
        Task<KanbanColumnResponseDto> UpdateKanbanColumnAsync(int id, UpdateKanbanColumnDto updateKanbanColumnDto);
        Task<bool> DeleteKanbanColumnAsync(int id);
    }
}