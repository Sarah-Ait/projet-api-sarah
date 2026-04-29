using backend.DTOs;

namespace backend.Interfaces
{
    public interface IKanbanColumnService
    {
        Task<List<KanbanColumnResponseDto>> GetAllKanbanColumnsAsync(int? userId = null);
        Task<KanbanColumnResponseDto> GetKanbanColumnByIdAsync(int id);
        Task<KanbanColumnResponseDto> CreateKanbanColumnAsync(CreateKanbanColumnDto createKanbanColumnDto);
        Task<KanbanColumnResponseDto> UpdateKanbanColumnAsync(int id, UpdateKanbanColumnDto updateKanbanColumnDto);
        Task<List<KanbanColumnResponseDto>> ReorderKanbanColumnsAsync(ReorderKanbanColumnsDto reorderKanbanColumnsDto);
        Task DeleteKanbanColumnAsync(int id);
    }
}