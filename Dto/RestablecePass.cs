using System.ComponentModel.DataAnnotations;


namespace ApiInmobiliariaAnNaTe.Models;
public class RestablecerPass
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string NuevaContrasena { get; set; }
}