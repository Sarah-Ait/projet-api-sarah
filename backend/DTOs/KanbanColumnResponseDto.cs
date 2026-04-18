namespace backend.DTOs;

public class KanbanColumnResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public int UserId { get; set; }
}