using backend.DTOs;

namespace backend.Interfaces
{
    public interface ITicketService
    {
        Task<List<TicketResponseDto>> GetAllTicketsAsync(int? userId = null);
        Task<TicketResponseDto> GetTicketByIdAsync(int id);
        Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto createTicketDto);
        Task<TicketResponseDto> UpdateTicketAsync(int id, UpdateTicketDto updateTicketDto);
        Task<TicketResponseDto> MoveTicketAsync(int id, MoveTicketDto moveTicketDto);
        Task DeleteTicketAsync(int id);
    }
}