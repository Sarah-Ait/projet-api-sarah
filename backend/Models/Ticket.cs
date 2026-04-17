namespace backend.Models;

public class Ticket
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double TimeSpentHours { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int AssignedUserId { get; set; }

    public User? AssignedUser { get; set; }

    public int KanbanColumnId { get; set; }

    public KanbanColumn? KanbanColumn { get; set; }
}