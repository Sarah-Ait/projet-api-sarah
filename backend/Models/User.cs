namespace backend.Models;

public class User //ranger cette classe dans l espace logique backend.Mode
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Standard";
}