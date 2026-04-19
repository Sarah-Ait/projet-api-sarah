using backend.Interfaces;
using backend.Models;
using backend.DTOs;

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

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllAsync();
        }

        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _ticketRepository.GetByIdAsync(id);
        }

        public async Task<Ticket> CreateTicketAsync(CreateTicketDto createTicketDto)
        {
            var user = await _userRepository.GetByIdAsync(createTicketDto.AssignedUserId);
            if (user == null)
            {
                return null;
            }

            var kanbanColumn = await _kanbanColumnRepository.GetByIdAsync(createTicketDto.KanbanColumnId);
            if (kanbanColumn == null)
            {
                return null;
            }

            var ticket = new Ticket
            {
                Title = createTicketDto.Title,
                Description = createTicketDto.Description,
                TimeSpentHours = createTicketDto.TimeSpentHours,
                AssignedUserId = createTicketDto.AssignedUserId,
                KanbanColumnId = createTicketDto.KanbanColumnId
            };

            return await _ticketRepository.CreateAsync(ticket);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(id);

            if (existingTicket == null)
            {
                return false;
            }

            await _ticketRepository.DeleteAsync(id);
            return true;
        }
    }
}