using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class UpdateTicketDto
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public double TimeSpentHours { get; set; }
}
