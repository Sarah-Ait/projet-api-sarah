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
    public class KanbanColumnsController : ControllerBase
    {
        private readonly IKanbanColumnService _kanbanColumnService;

        public KanbanColumnsController(IKanbanColumnService kanbanColumnService)
        {
            _kanbanColumnService = kanbanColumnService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KanbanColumnResponseDto>>> GetAllKanbanColumns([FromQuery] int? userId = null)
        {
            var kanbanColumns = await _kanbanColumnService.GetAllKanbanColumnsAsync(userId);
            return Ok(kanbanColumns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KanbanColumnResponseDto>> GetKanbanColumnById(int id)
        {
            var kanbanColumn = await _kanbanColumnService.GetKanbanColumnByIdAsync(id);
            return Ok(kanbanColumn);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<KanbanColumnResponseDto>> CreateKanbanColumn(CreateKanbanColumnDto createKanbanColumnDto)
        {
            var createdKanbanColumn = await _kanbanColumnService.CreateKanbanColumnAsync(createKanbanColumnDto);
            return CreatedAtAction(nameof(GetKanbanColumnById), new { id = createdKanbanColumn.Id }, createdKanbanColumn);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<KanbanColumnResponseDto>> UpdateKanbanColumn(int id, UpdateKanbanColumnDto updateKanbanColumnDto)
        {
            var updatedKanbanColumn = await _kanbanColumnService.UpdateKanbanColumnAsync(id, updateKanbanColumnDto);
            return Ok(updatedKanbanColumn);
        }

        [HttpPut("reorder")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<List<KanbanColumnResponseDto>>> ReorderKanbanColumns(ReorderKanbanColumnsDto reorderKanbanColumnsDto)
        {
            var reorderedColumns = await _kanbanColumnService.ReorderKanbanColumnsAsync(reorderKanbanColumnsDto);
            return Ok(reorderedColumns);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteKanbanColumn(int id)
        {
            await _kanbanColumnService.DeleteKanbanColumnAsync(id);
            return NoContent();
        }
    }
}