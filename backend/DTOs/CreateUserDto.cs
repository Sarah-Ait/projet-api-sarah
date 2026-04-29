using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class CreateUserDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Le mot de passe doit contenir au moins une majuscule, une minuscule et un chiffre."
    )]
    public string Password { get; set; } = string.Empty;
}