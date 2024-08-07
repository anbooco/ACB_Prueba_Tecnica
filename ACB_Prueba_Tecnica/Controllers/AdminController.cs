using ACB_Prueba_Tecnica.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACB_Prueba_Tecnica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpPost("generate-token")]
        public IActionResult GenerateToken()
        {
            var token = Guid.NewGuid().ToString();
            TokenStore.AddToken(token);
            return Ok(new { Token = token });
        }
    }
}
