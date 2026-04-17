using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    public string PasswordHash { get; set; } = string.Empty;
}