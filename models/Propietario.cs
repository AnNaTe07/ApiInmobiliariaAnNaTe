namespace ApiInmobiliariaAnNaTe.Models;

public class Propietario
{
    public int Id { get; set; }
    public string? Apellido { get; set; }
    public string? Nombre { get; set; }
    public string? Dni { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Estado { get; set; }
    public string? Pass { get; set; }
    public string? Salt { get; set; }
    public string? Avatar { get; set; }
}