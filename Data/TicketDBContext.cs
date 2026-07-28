using Microsoft.EntityFrameworkCore;
using TicketsApi.Models;

namespace TicketsApi.Data;

public class TicketDbContext : DbContext
{
    public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
}