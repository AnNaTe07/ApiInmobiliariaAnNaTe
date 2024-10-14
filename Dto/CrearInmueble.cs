using Microsoft.AspNetCore.Http; // Para IFormFile
using ApiInmobiliariaAnNaTe.Models;

public class CrearInmuebleDto
{
    public UsoInmueble Uso { get; set; }
    public string Direccion { get; set; } = "";
    public int TipoId { get; set; }
    public int Ambientes { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public decimal Superficie { get; set; }
    public decimal Precio { get; set; }
    public int IdPropietario { get; set; }
    public IFormFile? Foto { get; set; } // Foto opcional
}
