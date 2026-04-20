using backend.DTOs;

namespace backend.Interfaces
{
    public interface ITicketService
    {
        Task<List<TicketResponseDto>> GetAllTicketsAsync();
        Task<TicketResponseDto?> GetTicketByIdAsync(int id);
        Task<TicketResponseDto?> CreateTicketAsync(CreateTicketDto createTicketDto);
        Task<bool> DeleteTicketAsync(int id);
    }
}