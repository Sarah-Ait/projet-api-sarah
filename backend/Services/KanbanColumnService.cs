using backend.Interfaces;
using backend.Models;
using backend.DTOs;
using backend.Exceptions;

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

        public async Task<KanbanColumnResponseDto> GetKanbanColumnByIdAsync(int id)
        {
            var kanbanColumn = await _kanbanColumnRepository.GetByIdAsync(id);
            
            if (kanbanColumn == null)
                throw new NotFoundException($"Kanban column with ID {id} not found");

            return MapToKanbanColumnResponseDto(kanbanColumn);
        }

        public async Task<KanbanColumnResponseDto> CreateKanbanColumnAsync(CreateKanbanColumnDto createKanbanColumnDto)
        {
            // Validation
            if (createKanbanColumnDto == null)
                throw new ValidationException("Kanban column data is required");

            if (string.IsNullOrWhiteSpace(createKanbanColumnDto.Name))
                throw new ValidationException("Kanban column name is required");

            var user = await _userRepository.GetByIdAsync(createKanbanColumnDto.UserId);

            if (user == null)
                throw new NotFoundException($"User with ID {createKanbanColumnDto.UserId} not found");

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
                throw new NotFoundException($"Kanban column with ID {id} not found");

            await _kanbanColumnRepository.DeleteAsync(id);
            return true;
        }
    }
}