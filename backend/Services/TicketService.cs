using backend.Interfaces;
using backend.Models;

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

        public async Task<Ticket?> CreateTicketAsync(Ticket ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket.Title))
            {
                return null;
            }

            if (ticket.Title.Length > 100)
            {
                return null;
            }
            var assignedUser = await _userRepository.GetByIdAsync(ticket.AssignedUserId);

            if (assignedUser == null)
            {
                return null;
            }

            var kanbanColumn = await _kanbanColumnRepository.GetByIdAsync(ticket.KanbanColumnId);

            if (kanbanColumn == null)
            {
                return null;
            }

            return await _ticketRepository.CreateAsync(ticket);
        }
    }
}