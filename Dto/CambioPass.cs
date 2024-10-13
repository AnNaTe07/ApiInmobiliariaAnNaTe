using System.ComponentModel.DataAnnotations;
using ApiInmobiliariaAnNaTe.Models;


namespace ApiInmobiliariaAnNaTe.Models;
public class CambioPass
{
    [DataType(DataType.Password)]
    public string PassActual { get; set; }
    [DataType(DataType.Password)]
    public string NuevoPass { get; set; }
}
