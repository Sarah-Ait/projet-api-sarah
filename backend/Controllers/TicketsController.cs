using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using backend.Constants;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        [HttpPut("{id}")]
        public async Task<ActionResult<TicketResponseDto>> UpdateTicket(int id, UpdateTicketDto updateTicketDto)
        {
            var updatedTicket = await _ticketService.UpdateTicketAsync(id, updateTicketDto);
            return Ok(updatedTicket);
        }

        [HttpPatch("{id}/move")]
        public async Task<ActionResult<TicketResponseDto>> MoveTicket(int id, MoveTicketDto moveTicketDto)
        {
            var movedTicket = await _ticketService.MoveTicketAsync(id, moveTicketDto);
            return Ok(movedTicket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return NoContent();
        }
    }
}