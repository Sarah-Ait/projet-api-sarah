using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController] // montre que cette classe gere des routes api
    [Route("api/[controller]")]
    public class KanbanColumnsController : ControllerBase // ControllerBase: classe ou on recupere des outils comme ok notfound..
    {
        private readonly IKanbanColumnService _kanbanColumnService;

        public KanbanColumnsController(IKanbanColumnService kanbanColumnService)
        {
            _kanbanColumnService = kanbanColumnService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KanbanColumnResponseDto>>> GetAllKanbanColumns()
        {
            var kanbanColumns = await _kanbanColumnService.GetAllKanbanColumnsAsync();

            var response = kanbanColumns.Select(kanbanColumn => new KanbanColumnResponseDto
            {
                Id = kanbanColumn.Id,
                Name = kanbanColumn.Name,
                Order = kanbanColumn.Order,
                UserId = kanbanColumn.UserId
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KanbanColumnResponseDto>> GetKanbanColumnById(int id)
        {
            var kanbanColumn = await _kanbanColumnService.GetKanbanColumnByIdAsync(id);

            if (kanbanColumn == null)
            {
                return NotFound();
            }

            var response = new KanbanColumnResponseDto
            {
                Id = kanbanColumn.Id,
                Name = kanbanColumn.Name,
                Order = kanbanColumn.Order,
                UserId = kanbanColumn.UserId
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<KanbanColumnResponseDto>> CreateKanbanColumn(CreateKanbanColumnDto createKanbanColumnDto)
        {
            var createdKanbanColumn = await _kanbanColumnService.CreateKanbanColumnAsync(createKanbanColumnDto);
            if (createdKanbanColumn == null)
            {
                return NotFound(new { message = "User not found" });
            }


            var response = new KanbanColumnResponseDto
            {
                Id = createdKanbanColumn.Id,
                Name = createdKanbanColumn.Name,
                Order = createdKanbanColumn.Order,
                UserId = createdKanbanColumn.UserId
            };

            return CreatedAtAction(nameof(GetKanbanColumnById), new { id = response.Id }, response);
        }
    }
}