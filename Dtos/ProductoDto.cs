using System.ComponentModel.DataAnnotations;

namespace TicketsApi.Dtos;

// Para LEER tickets (lo que devuelve la API)
public record ProductoDto(int Id, string Nombre, decimal Precio);

// Para CREAR un ticket (lo que el cliente envía)
public class CrearProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MinLength(5, ErrorMessage = "El nombre debe tener al menos 5 caracteres.")]
    public string Nombre { get; set; } = "";

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.00")]
    public decimal Precio { get; set; }
}

public class ModificaPrecioDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.00")]
    public decimal Precio { get; set; }
}