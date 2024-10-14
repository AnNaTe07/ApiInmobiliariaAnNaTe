using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiInmobiliariaAnNaTe.Models;
using ApiInmobiliariaAnNaTe.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;



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
    public async Task<IActionResult> Estado(int id)
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

    private async Task<string> UploadToFirebase(IFormFile file)
    {
        var bucketName = "appinmobiliaria-2d959.appspot.com"; //nombre de bucket
        var storageUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o?name=inmuebles/{Uri.EscapeDataString(file.FileName)}";

        using var httpClient = new HttpClient();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0; // Reinicia la posición del stream

        using var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        var response = await httpClient.PostAsync(storageUrl, content);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
            // Retorna la URL de acceso a la imagen
            return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/inmuebles%2F{Uri.EscapeDataString(file.FileName)}?alt=media";
        }

        throw new Exception($"Error al subir la imagen: {response.ReasonPhrase}");
    }

    [HttpPost("foto/{id}")]
    [Authorize]
    public async Task<IActionResult> Foto(IFormFile file, int id)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No se ha seleccionado ningún archivo.");
        }

        // Lógica para subir la imagen a Firebase
        string imageUrl;
        try
        {
            imageUrl = await UploadToFirebase(file);
        }
        catch (Exception e)
        {
            return BadRequest("Error al subir la imagen: " + e.Message);
        }

        // Guarda la URL en la base de datos
        var inmueble = await _context.Inmuebles.FindAsync(id);

        if (inmueble == null)
        {
            return NotFound("Inmueble no encontrado.");
        }

        inmueble.Foto = imageUrl;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest("Error al actualizar foto: " + e.Message);
        }

        return Ok(new { Url = imageUrl });
    }

    [HttpPost("crear")]
    [Authorize]
    public async Task<IActionResult> CrearInmuebleConFoto([FromForm] CrearInmuebleDto nuevoInmuebleDto)
    {
        if (nuevoInmuebleDto == null)
        {
            return BadRequest("Los datos del inmueble son incorrectos.");
        }

        // Crea la entidad Inmueble
        var inmueble = new Inmueble
        {
            Uso = nuevoInmuebleDto.Uso,
            Direccion = nuevoInmuebleDto.Direccion,
            TipoId = nuevoInmuebleDto.TipoId,
            Ambientes = nuevoInmuebleDto.Ambientes,
            Latitud = nuevoInmuebleDto.Latitud,
            Longitud = nuevoInmuebleDto.Longitud,
            Superficie = nuevoInmuebleDto.Superficie,
            Precio = nuevoInmuebleDto.Precio,
            IdPropietario = nuevoInmuebleDto.IdPropietario,
            Estado = false // por defecto deshabilitado
        };

        // Agrego el inmueble a la base de datos
        _context.Inmuebles.Add(inmueble);
        await _context.SaveChangesAsync();

        // Si se proporciona una foto,se sube
        if (nuevoInmuebleDto.Foto != null)
        {
            string imageUrl;
            try
            {
                imageUrl = await UploadToFirebase(nuevoInmuebleDto.Foto);
                inmueble.Foto = imageUrl; // Actualiza URL de la foto en el inmueble
                await _context.SaveChangesAsync(); // Guarda cambios en la base de datos
            }
            catch (Exception e)
            {
                return BadRequest("Error al subir la imagen: " + e.Message);
            }
        }

        // Retorna el inmueble creado
        return Ok(inmueble);
    }

    [HttpGet("contrato/{id}")]
    [Authorize]
    public async Task<IActionResult> ObtenerContratoPorInmueble(int id)
    {
        var contrato = await _context.Contratos
            .Include(c => c.Inqui) // Incluye el inquilino
            .Include(c => c.Inmu)  // Incluye el inmueble
            .FirstOrDefaultAsync(c => c.InmuebleId == id && DateTime.Now >= c.Desde && DateTime.Now <= c.Hasta);

        if (contrato == null)
        {
            return NotFound("No hay contrato activo para este inmueble.");
        }

        // Devolver el contrato con las propiedades relacionadas
        return Ok(new
        {
            contrato.Id,
            contrato.Desde,
            contrato.Hasta,
            contrato.Monto,
            InquilinoNombre = contrato.Inqui != null ? contrato.Inqui.NombreCompleto : "No disponible",
            DireccionInmueble = contrato.DireccionInmueble
        });
    }

    [HttpGet("contrato/{contratoId}/pagos")]
    [Authorize]
    public async Task<IActionResult> ObtenerPagosPorContrato(int contratoId)
    {
        // Busca los pagos asociados al contrato, incluyendo el contrato y el inmueble relacionado
        var pagos = await _context.Pagos
            .Include(p => p.Contrato) // Incluye el contrato
            .ThenInclude(c => c.Inmu) // Incluye el inmueble del contrato
            .Where(p => p.ContratoId == contratoId)
            .ToListAsync();

        if (pagos == null || !pagos.Any())
        {
            return NotFound("No se encontraron pagos para este contrato.");
        }

        // Proporciona la respuesta con la información de los pagos
        var pagosDto = pagos.Select(p => new
        {
            p.Id,
            p.Nro,
            p.Fecha,
            p.Monto,
            p.ContratoId,
            DireccionContrato = p.Contrato?.Inmu?.Direccion ?? "Dirección no disponible" // Usa la dirección del inmueble si está disponible
        }).ToList();

        return Ok(pagosDto);
    }


}