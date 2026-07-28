namespace TicketsApi.Models;

public enum EstadoTicket
{
    Abierto,
    Cerrado
}

public class Ticket
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public EstadoTicket Estado { get; set; }
}