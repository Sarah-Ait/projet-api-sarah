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

        private readonly ICurrentUserService _currentUserService;

        public KanbanColumnService(
            IKanbanColumnRepository kanbanColumnRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService)
        {
            _kanbanColumnRepository = kanbanColumnRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
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
            var columns = await _kanbanColumnRepository.GetAllAsync();

            if (!_currentUserService.IsAdmin)
            {
                columns = columns
                    .Where(column => column.UserId == _currentUserService.UserId)
                    .ToList();
            }

            return columns.Select(MapToKanbanColumnResponseDto).ToList();
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
            // Validation - Check if user exists (business rule)
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

        public async Task<KanbanColumnResponseDto> UpdateKanbanColumnAsync(int id, UpdateKanbanColumnDto updateKanbanColumnDto)
        {
            var existingKanbanColumn = await _kanbanColumnRepository.GetByIdAsync(id);

            if (existingKanbanColumn == null)
                throw new NotFoundException($"Kanban column with ID {id} not found");

            existingKanbanColumn.Name = updateKanbanColumnDto.Name;

            var updatedKanbanColumn = await _kanbanColumnRepository.UpdateAsync(existingKanbanColumn);
            return MapToKanbanColumnResponseDto(updatedKanbanColumn);
        }

        public async Task<List<KanbanColumnResponseDto>> ReorderKanbanColumnsAsync(ReorderKanbanColumnsDto reorderKanbanColumnsDto)
        {
            var ids = reorderKanbanColumnsDto.OrderedColumnIds;

            if (ids.Distinct().Count() != ids.Count)
                throw new ValidationException("La liste d'IDs contient des doublons.");

            var columns = await _kanbanColumnRepository.GetByIdsAsync(ids);

            if (columns.Count != ids.Count)
                throw new NotFoundException("Une ou plusieurs colonnes n'existent pas.");

            var distinctUserIds = columns.Select(c => c.UserId).Distinct().ToList();

            if (distinctUserIds.Count != 1)
                throw new ValidationException("Toutes les colonnes doivent appartenir au même utilisateur.");

            var columnsById = columns.ToDictionary(column => column.Id);

            for (int i = 0; i < ids.Count; i++)
            {
                columnsById[ids[i]].Order = i + 1;
            }

            await _kanbanColumnRepository.UpdateRangeAsync(columns);

            return columns
                .OrderBy(column => column.Order)
                .Select(MapToKanbanColumnResponseDto)
                .ToList();
        }

        public async Task<bool> DeleteKanbanColumnAsync(int id)
        {
            var existingKanbanColumn = await _kanbanColumnRepository.GetByIdAsync(id);

            if (existingKanbanColumn == null)
                throw new NotFoundException($"Kanban column with ID {id} not found");
            if (!_currentUserService.IsAdmin && existingKanbanColumn.UserId != _currentUserService.UserId)
                throw new ForbiddenException("Vous ne pouvez pas supprimer cette colonne.");

            await _kanbanColumnRepository.DeleteAsync(id);
            return true;
        }
    }
}