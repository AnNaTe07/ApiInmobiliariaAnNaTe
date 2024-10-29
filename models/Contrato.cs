using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ApiInmobiliariaAnNaTe.Models;

public class Contrato
{
    [Display(Name = "Codigo")]
    public int Id { get; set; }
    public DateTime Desde { get; set; }
    public DateTime Hasta { get; set; }
    public decimal Monto { get; set; }
    public int InquilinoId { get; set; }
    [ForeignKey(nameof(InquilinoId))]
    public Inquilino? Inqui { get; set; }
    public int InmuebleId { get; set; }
    [ForeignKey(nameof(InmuebleId))]
    public Inmueble? Inmu { get; set; }

    public string DireccionInmueble => Inmu != null ? Inmu.Direccion : "Dirección no disponible";

    /*  public bool EstaActivo()
     {
         return DateTime.Now >= Desde && DateTime.Now <= Hasta;
     } */
}