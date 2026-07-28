using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketsApi.Models;
using TicketsApi.Data;
using TicketsApi.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace TicketsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly TicketDbContext _context;

    public TicketsController(TicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TicketDto>>> ObtenerTodos()
    {
        var tickets = await _context.Tickets
            .Select(t => new TicketDto(t.Id, t.Titulo, t.Estado.ToString()))
            .ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> ObtenerPorId(int id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound(new { mensaje = $"El ticket #{ id } no se encuentra"});
        }

        var dto = new TicketDto(ticket.Id, ticket.Titulo, ticket.Estado.ToString());
        return Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TicketDto>> Crear(CrearTicketDto datos)
    {
        var nuevoTicket = new Ticket
        {
            Titulo = datos.Titulo,
            Estado = EstadoTicket.Abierto
        };

        _context.Tickets.Add(nuevoTicket);
        await _context.SaveChangesAsync();

        var dto = new TicketDto(nuevoTicket.Id, nuevoTicket.Titulo, nuevoTicket.Estado.ToString());
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoTicket.Id }, dto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> CambiarEstado(int id, [FromBody] EstadoTicket nuevoEstado)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound(new { mensaje = $"El ticket #{ id } no se encuentra para cambiar su estado"});
        }

        ticket.Estado = nuevoEstado;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Eliminar(int id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound(new { mensaje = $"El ticket #{ id } no se encuentra para eliminar"});
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}