namespace backend.Models;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Standard";

    public List<KanbanColumn> KanbanColumns { get; set; } = new();

    public List<Ticket> AssignedTickets { get; set; } = new();
}