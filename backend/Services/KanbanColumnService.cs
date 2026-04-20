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

        private KanbanColumnResponseDto MapToKanbanColumnResponseDto(KanbanColumn kanbanColumn)
        {
            return new KanbanColumnResponseDto
            {
                Id = kanbanColumn.Id,
                Name = kanbanColumn.Name,
                Order = kanbanColumn.Order,
                UserId = kanbanColumn.UserId
            };
        }

        public async Task<List<KanbanColumnResponseDto>> GetAllKanbanColumnsAsync()
        {
            var kanbanColumns = await _kanbanColumnRepository.GetAllAsync();
            return kanbanColumns.Select(MapToKanbanColumnResponseDto).ToList();
        }

        public async Task<KanbanColumnResponseDto?> GetKanbanColumnByIdAsync(int id)
        {
            var kanbanColumn = await _kanbanColumnRepository.GetByIdAsync(id);
            
            if (kanbanColumn == null)
                return null;

            return MapToKanbanColumnResponseDto(kanbanColumn);
        }

        public async Task<KanbanColumnResponseDto> CreateKanbanColumnAsync(CreateKanbanColumnDto createKanbanColumnDto)
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

            var createdKanbanColumn = await _kanbanColumnRepository.CreateAsync(kanbanColumn);
            return MapToKanbanColumnResponseDto(createdKanbanColumn);
        }

        public async Task<bool> DeleteKanbanColumnAsync(int id)
        {
            var existingKanbanColumn = await _kanbanColumnRepository.GetByIdAsync(id);

            if (existingKanbanColumn == null)
            {
                return false;
            }

            await _kanbanColumnRepository.DeleteAsync(id);
            return true;
        }
    }
}