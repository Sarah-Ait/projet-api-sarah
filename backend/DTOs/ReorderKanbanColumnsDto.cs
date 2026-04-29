using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class ReorderKanbanColumnsDto
{
    [Required]
    [MinLength(1)]
    public List<int> OrderedColumnIds { get; set; } = new();
}
