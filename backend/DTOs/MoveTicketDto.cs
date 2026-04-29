using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class MoveTicketDto
{
    [Range(1, int.MaxValue)]
    public int TargetColumnId { get; set; }
}
