using backend.Models;

namespace backend.Interfaces
{
    public interface ITicketRepository
    {
        Task<List<Ticket>> GetAllAsync();
        Task<List<Ticket>> GetByUserIdAsync(int userId);
        Task<Ticket?> GetByIdAsync(int id);
        Task<Ticket> CreateAsync(Ticket ticket);
        Task<Ticket> UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);
    }
}