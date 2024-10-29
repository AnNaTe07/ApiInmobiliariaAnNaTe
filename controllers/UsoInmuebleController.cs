using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiInmobiliariaAnNaTe.Models;
namespace ApiInmobiliariaAnNaTe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsoInmuebleController : ControllerBase
{
    private readonly DataContext _context;

    public UsoInmuebleController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var usos = await _context.UsoInmuebles.ToListAsync();
        return Ok(usos);
    }
}