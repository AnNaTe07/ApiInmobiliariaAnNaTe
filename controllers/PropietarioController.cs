using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ApiInmobiliariaAnNaTe.Models;
using ApiInmobiliariaAnNaTe.utils;
using System.Net.Http.Headers;
using Google.Cloud.Storage.V1;
using ApiInmobiliariaAnNaTe.Services;
using Newtonsoft.Json;

namespace ApiInmobiliariaAnNaTe.Controllers;



[Route("api/propietario")]
[ApiController]
public class PropietarioController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger<PropietarioController> _logger;
    private readonly StorageClient _storageClient;
    private readonly Email _emailService;


    public PropietarioController(DataContext context, ILogger<PropietarioController> logger, StorageClient storageClient, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _storageClient = storageClient;
        _emailService = new Email(configuration);
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] Login login)
    {
        Console.WriteLine($"Correo: {login.Email}, Clave: {login.Pass}");

        var propietario = _context.Propietarios
       .FirstOrDefault(p => p.Email == login.Email);

        if (propietario == null)
        {
            return Unauthorized("Credenciales inválidas.");
        }
        //Console.WriteLine($"Nombre: {propietario.Nombre}, ID: {propietario.Id}, Email: {propietario.Email}, Estado: {propietario.Estado}");


        // Verifico pass
        if (!PasswordUtils.VerifyPassword(login.Pass, propietario.Pass, propietario.Salt))
        {
            return Unauthorized("Credenciales inválidas.");
        }
        Console.WriteLine($"ID del propietario: {propietario.Id}");

        // Genera el token JWT
        var token = GenerateJwtToken(propietario);
        return Ok(token);
    }

    private string GenerateJwtToken(Propietario propietario)
    {
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, propietario.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Email, propietario.Email),
        new Claim(ClaimTypes.NameIdentifier, propietario.Id.ToString()),
        new Claim("FullName", propietario.Nombre + " " + propietario.Apellido),
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("musculodenombremaslargo_esternocleidomastoideo"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "MiApp",
            audience: "LosUsuarios",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var propietario = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

            if (propietario == null)
            {
                return NotFound("No se encontró el perfil.");
            }

            return Ok(propietario);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al obtener los datos del perfil."); // Registro del error
            return StatusCode(500, "Error al obtener los datos del perfil.");
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update(Propietario propietario)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var propietarioLogin = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

        if (propietarioLogin == null || !User.Identity.IsAuthenticated)
        {
            return BadRequest("Datos incorrectos");
        }

        if (propietario != null)
        {
            propietarioLogin.Nombre = propietario.Nombre;
            propietarioLogin.Apellido = propietario.Apellido;
            propietarioLogin.Dni = propietario.Dni;
            propietarioLogin.Telefono = propietario.Telefono;
            propietarioLogin.Email = propietario.Email;

            _context.Propietarios.Update(propietarioLogin);

            try
            {
                _context.SaveChanges();

                return Ok(propietarioLogin);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        return BadRequest("No se pudo actualizar");
    }

    private async Task<string> UploadToFirebase(IFormFile file, string propietarioId)
    {
        var bucketName = "appinmobiliaria-2d959.appspot.com";
        var uniqueFileName = $"avatars/{propietarioId}_avatar.jpg"; // Uso el ID del propietario
        var storageUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o?name={uniqueFileName}";

        using var httpClient = new HttpClient();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

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


    [HttpPatch("avatar")]
    [Authorize]
    public async Task<IActionResult> Avatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No se ha seleccionado ningún archivo.");
        }

        string imageUrl;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var propietario = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

        if (propietario == null)
        {
            return NotFound("Propietario no encontrado.");
        }

        // ID del propietario para crear el nombre del archivo
        string propietarioId = propietario.Id.ToString();

        try
        {
            imageUrl = await UploadToFirebase(file, propietarioId);
        }
        catch (Exception e)
        {
            return BadRequest("Error al subir la imagen: " + e.Message);
        }

        propietario.Avatar = imageUrl;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest("Error al actualizar el avatar: " + e.Message);
        }

        return Ok(new { Url = imageUrl });
    }




    [HttpDelete("avatar")]
    [Authorize]
    public async Task<IActionResult> DeleteAvatar()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var propietario = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

        if (propietario == null)
        {
            return NotFound("Propietario no encontrado.");
        }

        var avatarUri = new Uri(propietario.Avatar);
        var objectName = $"avatars/{Path.GetFileName(avatarUri.LocalPath)}";

        Console.WriteLine($"Intentando eliminar el objeto: {objectName}");

        try
        {
            await _storageClient.DeleteObjectAsync("appinmobiliaria-2d959.appspot.com", objectName);
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound("El avatar no se encontró en Firebase Storage.");
        }

        propietario.Avatar = null;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest("Error al eliminar el avatar: " + e.Message);
        }

        return Ok("Avatar eliminado correctamente.");
    }

    [HttpPatch("pass")]
    [Authorize]
    public async Task<IActionResult> CambiarPass([FromBody] CambioPass pass)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var propietario = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

        if (propietario == null)
        {
            return NotFound("Propietario no encontrado.");
        }

        if (pass == null || string.IsNullOrEmpty(pass.PassActual) || string.IsNullOrEmpty(pass.NuevoPass))
        {
            return BadRequest("Los campos de contraseña no pueden estar vacíos.");
        }


        // Verifico el pass actual
        if (!PasswordUtils.VerifyPassword(pass.PassActual, propietario.Pass, propietario.Salt))
        {
            return BadRequest("La contraseña actual es incorrecta.");
        }

        // Hashea el nuevo pass 
        var (nuevoPassHash, nuevaSalt) = PasswordUtils.HashPassword(pass.NuevoPass);

        // Actualiza el pass y la sal en la base de datos
        propietario.Pass = nuevoPassHash;
        propietario.Salt = nuevaSalt;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest("Error al cambiar la contraseña: " + e.Message);
        }

        return Ok("Contraseña cambiada correctamente.");
    }


    [HttpPost("olvidoPass")]
    public async Task<IActionResult> OlvidoPass([FromBody] OlvidaPass dto)
    {
        var propietario = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == dto.Email);

        if (propietario == null)
        {
            return NotFound("Email no encontrado.");
        }

        // Genera el token
        var token = GenerateJwtToken(propietario);

        // Enlace para restablecer el pass
        var resetLink = $"http://192.168.1.2:5000/api/propietario/restablecePass?token={token}&email={dto.Email}";

        //envia el correo

        await _emailService.SendResetPasswordEmail(dto.Email, token); // Uso el método de la clase Email

        return Ok("Se ha enviado un correo para restablecer la contraseña.");
    }



    [HttpPost("restablecePass")]
    public async Task<IActionResult> RestablecerPass([FromBody] RestablecePass dto)
    {
        // para obtener el token del encabezado de autorización
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var propietario = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == dto.Email);

        if (propietario == null)
        {
            return NotFound("Email no encontrado.");
        }

        // Verifica si el token es válido
        if (!IsTokenValid(dto.Token))
        {
            return BadRequest("Token no válido o ha expirado.");
        }

        // Hashea el nuevo pass
        var (nuevoPassHash, nuevaSalt) = PasswordUtils.HashPassword(dto.NuevaContrasena);

        propietario.Pass = nuevoPassHash;
        propietario.Salt = nuevaSalt;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest("Error al restablecer la contraseña: " + e.Message);
        }

        return Ok("Contraseña restablecida correctamente.");
    }


    private bool IsTokenValid(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
        {
            return false; // Token no válido
        }

        // Verificar la fecha de expiración
        var expirationDate = jwtToken.ValidTo;
        return expirationDate > DateTime.UtcNow; // Compara con la hora actual en UTC
    }


}