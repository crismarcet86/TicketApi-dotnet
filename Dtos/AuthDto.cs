using System.ComponentModel.DataAnnotations;

namespace TicketsApi.Dtos;

public class RegistroDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Password { get; set; } = "";
}

public class LoginDto
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public record TokenResponseDto(string Token);