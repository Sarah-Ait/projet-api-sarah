using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class CreateKanbanColumnDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Order { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}