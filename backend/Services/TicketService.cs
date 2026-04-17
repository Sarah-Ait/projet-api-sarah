using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
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

            return await _ticketRepository.CreateAsync(ticket);
        }
    }
}