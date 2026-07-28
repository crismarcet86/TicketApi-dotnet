using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketsApi.Data;
using TicketsApi.Dtos;
using TicketsApi.Models;

namespace TicketsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TicketDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(TicketDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("registro")]
    public async Task<ActionResult> Registro(RegistroDto datos)
    {
        var yaExiste = await _context.Usuarios.AnyAsync(u => u.Email == datos.Email);
        if (yaExiste)
        {
            return BadRequest(new { mensaje = "Ya existe un usuario con ese email." });
        }

        var usuario = new Usuario
        {
            Email = datos.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(datos.Password)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Usuario registrado con éxito." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto datos)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == datos.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(datos.Password, usuario.PasswordHash))
        {
            return Unauthorized(new { mensaje = "Credenciales inválidas." });
        }

        var token = GenerarToken(usuario);
        return Ok(new TokenResponseDto(token));
    }

    private string GenerarToken(Usuario usuario)
    {
        var jwtKey = _config["Jwt:Key"]!;
        var jwtIssuer = _config["Jwt:Issuer"]!;

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}