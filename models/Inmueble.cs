using System.ComponentModel.DataAnnotations.Schema;

namespace ApiInmobiliariaAnNaTe.Models;

public class Inmueble
{
    public int Id { get; set; }

    [ForeignKey(nameof(UsoInmuebleId))]
    public int UsoInmuebleId { get; set; }
    public UsoInmueble UsoInmueble { get; set; }
    public string Direccion { get; set; } = "";

    [ForeignKey(nameof(TipoId))]
    public int TipoId { get; set; }
    public Tipo Tipo { get; set; }
    public int Ambientes { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public decimal Superficie { get; set; }
    public decimal Precio { get; set; }
    public int IdPropietario { get; set; }
    [ForeignKey(nameof(IdPropietario))]
    public Propietario PropietarioInmueble { get; set; }
    public bool Estado { get; set; } = false;
    public String Foto { get; set; } = "";
    [NotMapped]
    public IFormFile FotoFile { get; set; } // Para recibir la foto


}