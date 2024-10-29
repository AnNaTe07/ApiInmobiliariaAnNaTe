using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiInmobiliariaAnNaTe.Models;
namespace ApiInmobiliariaAnNaTe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoController : ControllerBase
{
    private readonly DataContext _context;

    public TipoController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var tipos = await _context.Tipos.ToListAsync();
        return Ok(tipos);
    }
}