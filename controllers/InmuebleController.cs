using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiInmobiliariaAnNaTe.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace ApiInmobiliariaAnNaTe.Controllers;

[ApiController]
[Route("api/inmueble")]
public class InmuebleController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger<InmuebleController> _logger;

    public InmuebleController(DataContext context, ILogger<InmuebleController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("get")]
    [Authorize]

    public async Task<IActionResult> GetInmuebles()
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized("No se pudo obtener el correo del propietario.");

            var inmuebles = await _context.Inmuebles
                .Include(i => i.Tipo)
                .Include(i => i.PropietarioInmueble)
                .Where(i => i.PropietarioInmueble.Email == email)
                .ToListAsync();


            if (inmuebles == null || !inmuebles.Any())
                return NotFound("No se encontraron inmuebles.");

            return Ok(inmuebles);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("estado/{id}")]
    [Authorize]
    public async Task<IActionResult> Put(int id)
    {
        var usuario = User.FindFirstValue(ClaimTypes.Email);

        if (usuario == null) return Unauthorized("Token no válido");

        // Busca el inmueble asociado al propietario
        var inmueble = await _context.Inmuebles
            .SingleOrDefaultAsync(e => e.Id == id && e.PropietarioInmueble.Email == usuario);

        if (inmueble == null) return NotFound("Inmueble no encontrado o acceso denegado");

        // Cambia el estado del inmueble
        inmueble.Estado = !inmueble.Estado;

        // Guarda cambios
        await _context.SaveChangesAsync();

        return Ok(inmueble);
    }

}