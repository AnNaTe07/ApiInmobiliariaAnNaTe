using System.ComponentModel.DataAnnotations;


namespace ApiInmobiliariaAnNaTe.Models;
public class RestablecePass
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string NuevaContrasena { get; set; }
}