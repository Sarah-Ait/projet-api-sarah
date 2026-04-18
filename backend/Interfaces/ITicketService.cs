using backend.Models;
using backend.DTOs;

namespace backend.Interfaces
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetAllTicketsAsync();
        Task<Ticket?> GetTicketByIdAsync(int id);
        Task<Ticket?> CreateTicketAsync(CreateTicketDto createTicketDto);
    }
}