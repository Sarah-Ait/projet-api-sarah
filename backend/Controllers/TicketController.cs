using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.Models;

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
        public async Task<ActionResult<List<Ticket>>> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound(new { message = "Ticket non trouvé" });
            }

            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket)
        {
            var createdTicket = await _ticketService.CreateTicketAsync(ticket);

            if (createdTicket == null)
            {
                return BadRequest(new { message = "Données du ticket invalides" });
            }

            return CreatedAtAction(nameof(GetTicketById), new { id = createdTicket.Id }, createdTicket);
        }
    }
}