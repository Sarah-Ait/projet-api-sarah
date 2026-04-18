using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.Models;
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
            if (createdTicket == null)
            {
                return NotFound(new { message = "Assigned user or kanban column not found" });
            }

            var response = tickets.Select(ticket => new TicketResponseDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                TimeSpentHours = ticket.TimeSpentHours,
                AssignedUserId = ticket.AssignedUserId,
                KanbanColumnId = ticket.KanbanColumnId
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponseDto>> GetTicketById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var response = new TicketResponseDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                TimeSpentHours = ticket.TimeSpentHours,
                AssignedUserId = ticket.AssignedUserId,
                KanbanColumnId = ticket.KanbanColumnId
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponseDto>> CreateTicket(CreateTicketDto createTicketDto)
        {
            var createdTicket = await _ticketService.CreateTicketAsync(createTicketDto);

            var response = new TicketResponseDto
            {
                Id = createdTicket.Id,
                Title = createdTicket.Title,
                Description = createdTicket.Description,
                TimeSpentHours = createdTicket.TimeSpentHours,
                AssignedUserId = createdTicket.AssignedUserId,
                KanbanColumnId = createdTicket.KanbanColumnId
            };

            return CreatedAtAction(nameof(GetTicketById), new { id = response.Id }, response);
        }
    }
}