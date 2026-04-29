using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class CreateTicketDto
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public double TimeSpentHours { get; set; }

    [Range(1, int.MaxValue)]
    public int KanbanColumnId { get; set; }
}