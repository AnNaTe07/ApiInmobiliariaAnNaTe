using Microsoft.AspNetCore.Mvc;
using ApiInmobiliariaAnNaTe.Models;

namespace ApiInmobiliariaAnNaTe.Controllers;

[ApiController]
[Route("api/inquilino")]
public class InquilinoController : ControllerBase
{
    [HttpGet]
    public IActionResult GetInquilinos()
    {
        return Ok();
    }
}