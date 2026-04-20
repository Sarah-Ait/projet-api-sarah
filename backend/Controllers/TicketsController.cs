using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponseDto>> GetTicketById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponseDto>> CreateTicket(CreateTicketDto createTicketDto)
        {
            var createdTicket = await _ticketService.CreateTicketAsync(createTicketDto);
            return CreatedAtAction(nameof(GetTicketById), new { id = createdTicket.Id }, createdTicket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return NoContent();
        }
    }
}