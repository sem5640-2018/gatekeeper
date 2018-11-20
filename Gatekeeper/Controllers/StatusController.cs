using Microsoft.AspNetCore.Mvc;

namespace Gatekeeper.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        // GET: /Status
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
