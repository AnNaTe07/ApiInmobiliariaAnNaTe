using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ApiInmobiliariaAnNaTe.Models;


public class Pago
{
    public int? Id { get; set; }

    [Display(Name = "Numero de pago")]
    public int Nro { get; set; }
    public DateTime Fecha { get; set; }
    public Decimal Monto { get; set; }
    public int ContratoId { get; set; }
    [ForeignKey(nameof(ContratoId))]
    public Contrato Contrato { get; set; }

    public String? DireccionContrato => Contrato?.Inmu?.Direccion ?? "Dirección no disponible";

}
