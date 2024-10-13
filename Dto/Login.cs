using System.ComponentModel.DataAnnotations;

namespace ApiInmobiliariaAnNaTe.Models;

public class Login
{
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string Pass { get; set; }
}