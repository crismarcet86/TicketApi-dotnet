using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketsApi.Models;
using TicketsApi.Data;
using TicketsApi.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace TicketsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly TicketDbContext _context;

    public ProductosController(TicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductoDto>>> ObtenerTodos()
    {
        var producto = await _context.Productos
            .Select(t => new ProductoDto(t.Id, t.Nombre, t.Precio))
            .ToListAsync();

        return Ok(producto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoDto>> ObtenerPorId(int id)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(t => t.Id == id);

        if (producto == null)
        {
            return NotFound(new { mensaje = $"El producto #{ id } no se encuentra"});
        }

        var dto = new ProductoDto(producto.Id, producto.Nombre, producto.Precio);
        return Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProductoDto>> Crear(CrearProductoDto datos)
    {
        var nuevoProducto = new Producto
        {
            Nombre = datos.Nombre,
            Precio = datos.Precio
        };

        var producto = await _context.Productos.FirstOrDefaultAsync(t => t.Nombre == datos.Nombre);

        if (producto == null)
        {
            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync();
        } else
        {
            return NotFound($"El producto {datos.Nombre} ya fue ingresado anteriormente");
        }

        var dto = new ProductoDto(nuevoProducto.Id, nuevoProducto.Nombre, nuevoProducto.Precio);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoProducto.Id }, dto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> CambiarPrecio(int id, ModificaPrecioDto dato)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(t => t.Id == id);

        if (producto == null)
        {
            return NotFound(new { mensaje = $"El producto #{ id } no se encuentra para cambiar el precio"});
        }

        producto.Precio = dato.Precio;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Eliminar(int id)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(t => t.Id == id);

        if (producto == null)
        {
            return NotFound(new { mensaje = $"El producto #{ id } no se encuentra para eliminar"});
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}