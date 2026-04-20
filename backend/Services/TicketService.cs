using backend.Interfaces;
using backend.Models;
using backend.DTOs;
using backend.Exceptions;

namespace backend.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IKanbanColumnRepository _kanbanColumnRepository;

        public TicketService(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            IKanbanColumnRepository kanbanColumnRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _kanbanColumnRepository = kanbanColumnRepository;
        }

        private TicketResponseDto MapToTicketResponseDto(Ticket ticket)
        {
            return new TicketResponseDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                TimeSpentHours = ticket.TimeSpentHours,
                AssignedUserId = ticket.AssignedUserId,
                KanbanColumnId = ticket.KanbanColumnId
            };
        }

        public async Task<List<TicketResponseDto>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
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
            // Validation - Check if user and kanban column exist (business rules)
            var user = await _userRepository.GetByIdAsync(createTicketDto.AssignedUserId);
            if (user == null)
                throw new NotFoundException($"User with ID {createTicketDto.AssignedUserId} not found");

            var kanbanColumn = await _kanbanColumnRepository.GetByIdAsync(createTicketDto.KanbanColumnId);
            if (kanbanColumn == null)
                throw new NotFoundException($"Kanban column with ID {createTicketDto.KanbanColumnId} not found");

            var ticket = new Ticket
            {
                Title = createTicketDto.Title,
                Description = createTicketDto.Description,
                TimeSpentHours = createTicketDto.TimeSpentHours,
                AssignedUserId = createTicketDto.AssignedUserId,
                KanbanColumnId = createTicketDto.KanbanColumnId
            };

            var createdTicket = await _ticketRepository.CreateAsync(ticket);
            return MapToTicketResponseDto(createdTicket);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(id);

            if (existingTicket == null)
                throw new NotFoundException($"Ticket with ID {id} not found");

            await _ticketRepository.DeleteAsync(id);
            return true;
        }
    }
}