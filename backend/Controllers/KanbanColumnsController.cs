using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.Models;

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
        public async Task<ActionResult<List<KanbanColumn>>> GetAllKanbanColumns()
        {
            var kanbanColumns = await _kanbanColumnService.GetAllKanbanColumnsAsync();
            return Ok(kanbanColumns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KanbanColumn>> GetKanbanColumnById(int id)
        {
            var kanbanColumn = await _kanbanColumnService.GetKanbanColumnByIdAsync(id);

            if (kanbanColumn == null)
            {
                return NotFound(new { message = "Colonne kanban non trouvée" });
            }

            return Ok(kanbanColumn);
        }

        [HttpPost]
        public async Task<ActionResult<KanbanColumn>> CreateKanbanColumn(KanbanColumn kanbanColumn)
        {
            var createdKanbanColumn = await _kanbanColumnService.CreateKanbanColumnAsync(kanbanColumn);

            if (createdKanbanColumn == null)
            {
                return BadRequest(new { message = "Données de colonne kanban invalides" });
            }

            return CreatedAtAction(nameof(GetKanbanColumnById), new { id = createdKanbanColumn.Id }, createdKanbanColumn);
        }
    }
}