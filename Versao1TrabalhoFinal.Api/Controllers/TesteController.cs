using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TesteController : ControllerBase
    {
        [HttpGet("publico")]
        public IActionResult Publico()
        {
            return Ok(new { message = "Endpoint público." });
        }

        [Authorize]
        [HttpGet("privado")]
        public IActionResult Privado()
        {
            return Ok(new
            {
                message = "Endpoint privado com JWT.",
                user = User.Identity?.Name
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok(new { message = "Endpoint exclusivo para Admin." });
        }
    }
}