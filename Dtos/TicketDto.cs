using System.ComponentModel.DataAnnotations;

namespace TicketsApi.Dtos;

// Para LEER tickets (lo que devuelve la API)
public record TicketDto(int Id, string Titulo, string Estado);

// Para CREAR un ticket (lo que el cliente envía)
public class CrearTicketDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [MinLength(5, ErrorMessage = "El título debe tener al menos 5 caracteres.")]
    public string Titulo { get; set; } = "";
}