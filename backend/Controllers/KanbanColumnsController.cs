using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanbanColumnsController : ControllerBase
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
            return Ok(kanbanColumns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KanbanColumnResponseDto>> GetKanbanColumnById(int id)
        {
            var kanbanColumn = await _kanbanColumnService.GetKanbanColumnByIdAsync(id);

            if (kanbanColumn == null)
            {
                return NotFound();
            }

            return Ok(kanbanColumn);
        }

        [HttpPost]
        public async Task<ActionResult<KanbanColumnResponseDto>> CreateKanbanColumn(CreateKanbanColumnDto createKanbanColumnDto)
        {
            var createdKanbanColumn = await _kanbanColumnService.CreateKanbanColumnAsync(createKanbanColumnDto);
            if (createdKanbanColumn == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return CreatedAtAction(nameof(GetKanbanColumnById), new { id = createdKanbanColumn.Id }, createdKanbanColumn);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKanbanColumn(int id)
        {
            var deleted = await _kanbanColumnService.DeleteKanbanColumnAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Kanban column not found" });
            }

            return NoContent();
        }
    }
}