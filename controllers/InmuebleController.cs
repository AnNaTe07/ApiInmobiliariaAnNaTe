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
[Route("api/[controller]")]
public class InmuebleController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger<InmuebleController> _logger;

    public InmuebleController(DataContext context, ILogger<InmuebleController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ObtenerInmueblesDelPropietario()
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("No se pudo obtener el correo del propietario.");

            var inmuebles = await _context.Inmuebles
                .Include(i => i.Tipo)
                .Include(i => i.PropietarioInmueble)
                .Select(inmueble => new InmuebleContrato
                {
                    Inmueble = inmueble,
                    Contrato = _context.Contratos
                        .Where(c => c.InmuebleId == inmueble.Id)
                        .OrderByDescending(c => c.Desde)
                        .FirstOrDefault()
                })
                .ToListAsync();

            if (!inmuebles.Any())
                return NotFound("No se encontraron inmuebles.");

            return Ok(inmuebles);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("get/{id}")]
    [Authorize]
    public async Task<IActionResult> ObtenerInmueble(int id)
    {
        // Verifica que el usuario esté autenticado
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized("Usuario no autenticado.");
        }

        // Obtiene el correo electrónico del usuario autenticado
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var propietarioLogin = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

        if (propietarioLogin == null)
        {
            return BadRequest("Datos incorrectos.");
        }

        // Busca el inmueble que pertenece a este propietario
        var inmueble = await _context.Inmuebles
            .SingleOrDefaultAsync(i => i.Id == id && i.IdPropietario == propietarioLogin.Id);

        if (inmueble == null)
        {
            return NotFound("Inmueble no encontrado o no pertenece al propietario.");
        }

        // Retorna el inmueble encontrado
        return Ok(inmueble);
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

    private async Task<string> UploadToFirebase(IFormFile file, int inmuebleId)
    {
        var bucketName = "appinmobiliaria-2d959.appspot.com"; // nombre de bucket
        var uniqueFileName = $"inmuebles/inmueble_{inmuebleId}_{Uri.EscapeDataString(file.FileName)}";
        var storageUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o?name={uniqueFileName}";

        using var httpClient = new HttpClient();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0; // Reinicia la posición del stream

        using var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        var response = await httpClient.PostAsync(storageUrl, content);
        if (response.IsSuccessStatusCode)
        {
            return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(uniqueFileName)}?alt=media";
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new Exception($"Error al subir la imagen: {response.ReasonPhrase}, Detalle: {errorMessage}");
    }



    [HttpPatch("foto/{id}")]
    [Authorize]
    public async Task<IActionResult> Foto(IFormFile file, int id)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No se ha seleccionado ningún archivo.");
        }

        // Para subir la imagen a Firebase usando el ID del inmueble
        string imageUrl;
        try
        {
            imageUrl = await UploadToFirebase(file, id);
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
    public async Task<IActionResult> CrearInmuebleConFoto([FromForm] int usoInmuebleId, [FromForm] int tipoId, [FromForm] int ambientes,
    [FromForm] string direccion, [FromForm] decimal latitud, [FromForm] decimal longitud,
    [FromForm] decimal precio, [FromForm] decimal superficie, [FromForm] IFormFile foto)
    {
        // Verifico que el usuario esté autenticado
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized("Usuario no autenticado.");
        }

        // Obtengo el ID del propietario desde las claims
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var propietarioId = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

        if (propietarioId == null)
        {
            return BadRequest("Datos incorrectos.");
        }
        // Creo una nueva instancia de Inmueble
        var inmueble = new Inmueble
        {
            UsoInmuebleId = usoInmuebleId,
            Direccion = direccion,
            TipoId = tipoId,
            Ambientes = ambientes,
            Latitud = latitud,
            Longitud = longitud,
            Superficie = superficie,
            Precio = precio,
            IdPropietario = propietarioId.Id,
            Estado = false
        };

        // Agrego el inmueble a la base de datos primero para obtener su ID
        _context.Inmuebles.Add(inmueble);
        await _context.SaveChangesAsync();

        // Si se proporciona una foto...
        if (foto != null && foto.Length > 0)
        {
            try
            {
                var imageUrl = await UploadToFirebase(foto, inmueble.Id);
                inmueble.Foto = imageUrl; // URL de la foto
                await _context.SaveChangesAsync(); // Guardar el cambio de foto
            }
            catch (Exception e)
            {
                return BadRequest("Error al subir la imagen: " + e.Message);
            }
        }
        else
        {
            return BadRequest("No se ha proporcionado una imagen válida.");
        }

        return CreatedAtAction(nameof(CrearInmuebleConFoto), new { id = inmueble.Id }, inmueble);
    }

    [HttpGet("alquilados")]
    [Authorize]
    public async Task<IActionResult> InmueblesAlquilados()
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("No se pudo obtener el correo del propietario.");

            // Obtengo el ID del propietario
            var propietario = await _context.Propietarios
                .FirstOrDefaultAsync(p => p.Email == email);

            if (propietario == null)
                return NotFound("Propietario no encontrado.");

            // Obtengo contratos activos y también incluyo el tipo del inmueble
            var contratosActivos = await _context.Contratos
                .Where(c => c.Inmu.IdPropietario == propietario.Id && DateTime.Now >= c.Desde && DateTime.Now <= c.Hasta)
                .Include(c => c.Inqui) // Incluyo el inquilino
                .Include(c => c.Inmu)  // Incluyo el inmueble
                .ThenInclude(i => i.Tipo) //incluyo el tipo del inmueble asociado a inmueble
                .ToListAsync();

            // Combino inmuebles y contratos
            var inmuebleContratos = contratosActivos.Select(c => new InmuebleContrato
            {
                Inmueble = c.Inmu,
                Contrato = c
            }).ToList();

            if (!inmuebleContratos.Any())
                return NotFound("No se encontraron inmuebles alquilados para el propietario.");

            return Ok(inmuebleContratos);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);//aqui explota!!!
        }
    }

    [HttpGet("inquilino/{inmuebleId}")]
    [Authorize]
    public async Task<IActionResult> ObtenerInquilino(int inmuebleId)
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized("No se pudo obtener el correo del propietario.");

            // propietario logueado
            var propietario = await _context.Propietarios
                .FirstOrDefaultAsync(p => p.Email == email);

            if (propietario == null)
                return NotFound("Propietario no encontrado.");

            // fecha actual
            var fechaActual = DateTime.Now;

            // contrato activo para el inmueble especificado
            var contrato = await _context.Contratos
                .Include(c => c.Inqui) // Incluyo el inquilino
                .FirstOrDefaultAsync(c => c.InmuebleId == inmuebleId
                    && c.Desde <= fechaActual
                    && c.Hasta >= fechaActual);

            if (contrato == null)
                return NotFound("No se encontró un contrato activo para el inmueble especificado.");

            // Retorno la información del inquilino
            return Ok(contrato.Inqui);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [HttpGet("contrato/{id}")]
    [Authorize]
    public async Task<IActionResult> ObtenerContratoPorInmueble(int id)
    {
        var contrato = await _context.Contratos
            .Include(c => c.Inqui) // Incluyo el inquilino
            .Include(c => c.Inmu)  // Incluyo el inmueble
            .FirstOrDefaultAsync(c => c.InmuebleId == id && DateTime.Now >= c.Desde && DateTime.Now <= c.Hasta);

        if (contrato == null)
        {
            return NotFound("No hay contrato activo para este inmueble.");
        }

        // Retorna el contrato con las propiedades relacionadas
        return Ok(new
        {
            contrato.Id,
            contrato.Desde,
            contrato.Hasta,
            contrato.Monto,
            inquilino = contrato.Inqui != null ? contrato.Inqui.NombreCompleto : "No disponible",
            inmueble = contrato.DireccionInmueble
        });
    }

    [HttpGet("{contratoId}/pagos")]
    [Authorize]
    public async Task<IActionResult> ObtenerPagosPorContrato(int contratoId)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("No se pudo determinar el propietario.");
        }

        var propietarioIdInt = await _context.Propietarios
            .Where(x => x.Email == email)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (propietarioIdInt == 0) //si no se encontró el propietario
        {
            return NotFound("Propietario no encontrado.");
        }

        //inmueble asociado al contrato
        var inmuebleId = await _context.Contratos
            .Where(c => c.Id == contratoId)
            .Select(c => c.InmuebleId)
            .FirstOrDefaultAsync();

        if (inmuebleId == 0) //si no se encontró el inmueble
        {
            return NotFound("Inmueble no encontrado para el contrato.");
        }

        //pagos asociados al contrato
        var pagos = await _context.Pagos
            .Include(p => p.Contrato) // Incluye el contrato
            .ThenInclude(c => c.Inmu) // Incluye el inmueble del contrato
            .Where(p => p.ContratoId == contratoId && inmuebleId == p.Contrato.InmuebleId &&
                _context.Inmuebles
                    .Any(i => i.Id == inmuebleId && i.IdPropietario == propietarioIdInt)) // Verifica que el inmueble pertenezca al propietario
            .ToListAsync();

        if (!pagos.Any())
        {
            return NotFound("No se encontraron pagos para este contrato.");
        }

        var pagosDto = pagos.Select(p => new
        {
            p.Id,
            p.Nro,
            p.Fecha,
            p.Monto,
            p.ContratoId,
            Direccion = p.Contrato.Inmu.Direccion //dirección del inmueble
        }).ToList();

        return Ok(pagosDto);
    }

}