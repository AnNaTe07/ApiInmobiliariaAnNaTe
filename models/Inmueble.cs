using System.ComponentModel.DataAnnotations.Schema;

namespace ApiInmobiliariaAnNaTe.Models;

public enum UsoInmueble
{
    Comercial = 1,
    Residencial = 2
}
public class Inmueble
{
    public int Id { get; set; }
    public UsoInmueble Uso { get; set; }
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
    public bool Estado { get; set; } = true;
    public String Foto { get; set; } = "";

    public bool EstaAlquilado(List<Contrato> contratos)
    {
        return contratos.Any(c => c.InmuebleId == this.Id && DateTime.Now >= c.Desde && DateTime.Now <= c.Hasta);
    }
}