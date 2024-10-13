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
using MailKit.Net.Smtp;
using MimeKit;


using Newtonsoft.Json;

namespace ApiInmobiliariaAnNaTe.Controllers;


[ApiController]
[Route("api/propietario")]
public class PropietarioController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger<PropietarioController> _logger;
    private readonly StorageClient _storageClient;


    public PropietarioController(DataContext context, ILogger<PropietarioController> logger, StorageClient storageClient)
    {
        _context = context;
        _logger = logger;
        _storageClient = storageClient;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] Propietario propietario)
    {
        if (string.IsNullOrEmpty(propietario.Pass))
            return BadRequest("La contraseña es obligatoria.");

        // Verifico si el email ya está en uso
        if (_context.Propietarios.Any(p => p.Email == propietario.Email))
            return BadRequest("Email ya está en uso.");

        // Hashear la contraseña y obtener el salt
        var hashResult = PasswordUtils.HashPassword(propietario.Pass);
        propietario.Pass = hashResult.hashedPassword;
        propietario.Salt = hashResult.salt;

        // Verifico el valor del salt
        //Console.WriteLine($"Salt generado: {propietario.Salt}");

        try
        {
            _context.Propietarios.Add(propietario);
            _context.SaveChanges();
            return Ok("Registro exitoso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error interno del servidor: " + ex.Message);
        }
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


        // Verifica la contraseña
        if (!PasswordUtils.VerifyPassword(login.Pass, propietario.Pass, propietario.Salt))
        {
            return Unauthorized("Credenciales inválidas.");
        }
        Console.WriteLine($"ID del propietario: {propietario.Id}");

        // Genera el token JWT
        var token = GenerateJwtToken(propietario);
        return Ok(new { Token = token });
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
            _logger.LogError(e, "Error al obtener los datos del perfil."); // Registro el error
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

    private async Task<string> UploadToFirebase(IFormFile file)
    {
        var bucketName = "appinmobiliaria-2d959.appspot.com"; //nombre de bucket
        var storageUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o?name=avatars/{Uri.EscapeDataString(file.FileName)}";

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
            return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/avatars%2F{Uri.EscapeDataString(file.FileName)}?alt=media";
        }

        throw new Exception($"Error al subir la imagen: {response.ReasonPhrase}");
    }

    [HttpPost("avatar")]
    [Authorize]
    public async Task<IActionResult> Avatar(IFormFile file)
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
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var propietario = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

        if (propietario == null)
        {
            return NotFound("Propietario no encontrado.");
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
        var objectName = $"avatars/{Path.GetFileName(avatarUri.LocalPath)}"; // Usa LocalPath

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
    [HttpPost("pass")]
    [Authorize]
    public async Task<IActionResult> CambiarPass([FromBody] CambioPass pass)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var propietario = await _context.Propietarios.SingleOrDefaultAsync(x => x.Email == email);

        if (propietario == null)
        {
            return NotFound("Propietario no encontrado.");
        }

        // Verifica el pass actual
        if (!PasswordUtils.VerifyPassword(pass.PassActual, propietario.Pass, propietario.Salt))
        {
            return BadRequest("La contraseña actual es incorrecta.");
        }

        // Hashea la nueva contraseña 
        var (nuevoPassHash, nuevaSalt) = PasswordUtils.HashPassword(pass.NuevoPass);

        // Actualiza la contraseña y la sal en la base de datos
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

        //enlace para el restablecimiento de la contraseña
        var resetLink = $"http://localhost:5000/restablecerPass?token={token}&email={dto.Email}";

        // Crear y enviar el correo
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Andrea Natalia", "andreanataliatello@outlook.com"));
        message.To.Add(new MailboxAddress("", dto.Email));
        message.Subject = "Restablecimiento de contraseña";

        /*   message.Body = new TextPart("plain")
          {
              Text = $"Haz clic en el siguiente enlace para restablecer tu contraseña: {resetLink}"
          }; */
        message.Body = new TextPart("html")
        {
            Text = $@"
        <html>
        <body>
            <p>Haz solicitado restablecer tu contraseña? Si es así, haz clic en el siguiente botón para confirmar el restablecimiento de tu contraseña:</p>
            <a href='{resetLink}' style='padding: 10px; background-color: blue; color: white; text-decoration: none;'>Sí, restablecer contraseña</a>

            <p>Si no has solicitado restablecer tu contraseña, puedes ignorar este correo.</p>
        </body>
        </html>"
        };
        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("toony1717@gmail.com", "xsip fkbb oiqw zosc");//contraseña de aplicacion
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }

            return Ok("Se ha enviado un correo para restablecer la contraseña.");
        }
    }

    [HttpPost("restablecerPass")]
    public async Task<IActionResult> RestablecerPass([FromBody] RestablecerPass dto)
    {
        // para obtener el token del encabezado de autorización...
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

        // Hashea la nueva contraseña
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