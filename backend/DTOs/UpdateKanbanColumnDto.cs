using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class UpdateKanbanColumnDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
