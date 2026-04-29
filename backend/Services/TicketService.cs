using backend.Interfaces;
using backend.Models;
using backend.DTOs;
using backend.Exceptions;

namespace backend.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IKanbanColumnRepository _kanbanColumnRepository;
        private readonly ICurrentUserService _currentUserService;

        public TicketService(
            ITicketRepository ticketRepository,
            IKanbanColumnRepository kanbanColumnRepository,
            ICurrentUserService currentUserService)
        {
            _ticketRepository = ticketRepository;
            _kanbanColumnRepository = kanbanColumnRepository;
            _currentUserService = currentUserService;
        }

        private TicketResponseDto MapToTicketResponseDto(Ticket ticket)
        {
            return new TicketResponseDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                TimeSpentHours = ticket.TimeSpentHours,
                CreatedAt = ticket.CreatedAt,
                AdminNote = ticket.AdminNote,
                AssignedUserId = ticket.AssignedUserId,
                KanbanColumnId = ticket.KanbanColumnId
            };
        }

        public async Task<List<TicketResponseDto>> GetAllTicketsAsync(int? userId = null)
        {
            List<Ticket> tickets;

            if (_currentUserService.IsAdmin)
            {
                tickets = userId.HasValue
                    ? await _ticketRepository.GetByUserIdAsync(userId.Value)
                    : await _ticketRepository.GetAllAsync();
            }
            else
            {
                tickets = await _ticketRepository.GetByUserIdAsync(_currentUserService.UserId);
            }

            return tickets.Select(MapToTicketResponseDto).ToList();
        }

        public async Task<TicketResponseDto> GetTicketByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            
            if (ticket == null)
                throw new NotFoundException($"Ticket with ID {id} not found");

            return MapToTicketResponseDto(ticket);
        }

        public async Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto createTicketDto)
        {
            var kanbanColumn = await _kanbanColumnRepository.GetByIdAsync(createTicketDto.KanbanColumnId);

            if (kanbanColumn == null)
                throw new NotFoundException($"Kanban column with ID {createTicketDto.KanbanColumnId} not found");

            if (!_currentUserService.IsAdmin && kanbanColumn.UserId != _currentUserService.UserId)
                throw new ForbiddenException("Vous ne pouvez pas créer un ticket dans cette colonne.");

            var actingAsAdminForOther =
                _currentUserService.IsAdmin && kanbanColumn.UserId != _currentUserService.UserId;

            var ticket = new Ticket
            {
                Title = createTicketDto.Title,
                Description = createTicketDto.Description,
                TimeSpentHours = createTicketDto.TimeSpentHours,
                AssignedUserId = kanbanColumn.UserId,
                KanbanColumnId = createTicketDto.KanbanColumnId,
                AdminNote = actingAsAdminForOther ? "Créé par l'admin" : null
            };

            var createdTicket = await _ticketRepository.CreateAsync(ticket);
            return MapToTicketResponseDto(createdTicket);
        }

        public async Task<TicketResponseDto> UpdateTicketAsync(int id, UpdateTicketDto updateTicketDto)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(id);

            if (existingTicket == null)
                throw new NotFoundException($"Ticket with ID {id} not found");

            if (!_currentUserService.IsAdmin && existingTicket.AssignedUserId != _currentUserService.UserId)
                throw new ForbiddenException("Vous ne pouvez pas modifier ce ticket.");

            existingTicket.Title = updateTicketDto.Title;
            existingTicket.Description = updateTicketDto.Description;
            existingTicket.TimeSpentHours = updateTicketDto.TimeSpentHours;

            if (_currentUserService.IsAdmin && existingTicket.AssignedUserId != _currentUserService.UserId)
            {
                existingTicket.AdminNote = "Modifié par l'admin";
            }

            var updatedTicket = await _ticketRepository.UpdateAsync(existingTicket);
            return MapToTicketResponseDto(updatedTicket);
        }

        public async Task<TicketResponseDto> MoveTicketAsync(int id, MoveTicketDto moveTicketDto)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(id);

            if (existingTicket == null)
                throw new NotFoundException($"Ticket with ID {id} not found");

            if (!_currentUserService.IsAdmin && existingTicket.AssignedUserId != _currentUserService.UserId)
                throw new ForbiddenException("Vous ne pouvez pas déplacer ce ticket.");

            var targetColumn = await _kanbanColumnRepository.GetByIdAsync(moveTicketDto.TargetColumnId);

            if (targetColumn == null)
                throw new NotFoundException($"Kanban column with ID {moveTicketDto.TargetColumnId} not found");

            if (targetColumn.UserId != existingTicket.AssignedUserId)
                throw new ForbiddenException("La colonne cible n'appartient pas au propriétaire du ticket.");

            existingTicket.KanbanColumnId = moveTicketDto.TargetColumnId;

            if (_currentUserService.IsAdmin && existingTicket.AssignedUserId != _currentUserService.UserId)
            {
                existingTicket.AdminNote = "Déplacé par l'admin";
            }

            var movedTicket = await _ticketRepository.UpdateAsync(existingTicket);
            return MapToTicketResponseDto(movedTicket);
        }

        public async Task DeleteTicketAsync(int id)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(id);

            if (existingTicket == null)
                throw new NotFoundException($"Ticket with ID {id} not found");

            if (!_currentUserService.IsAdmin && existingTicket.AssignedUserId != _currentUserService.UserId)
                throw new ForbiddenException("Vous ne pouvez pas supprimer ce ticket.");

            await _ticketRepository.DeleteAsync(id);
        }
    }
}