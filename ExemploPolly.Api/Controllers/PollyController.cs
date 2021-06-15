using Microsoft.AspNetCore.Mvc;
using System;

namespace ExemploPolly.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PollyController : ControllerBase
    {
        [HttpGet("Numeros")]
        public IActionResult Numeros()
        {
            int percent = new Random().Next(1, 100);
            if (percent <= 50)
            {
                return Ok(new
                {
                    Numeros = new int[] { 1, 2, 3 }
                });
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
