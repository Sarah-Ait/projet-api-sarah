namespace backend.DTOs;

public class TicketResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double TimeSpentHours { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AdminNote { get; set; }
    public int AssignedUserId { get; set; }
    public int KanbanColumnId { get; set; }
}