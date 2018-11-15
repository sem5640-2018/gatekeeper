using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gatekeeper.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserManager<GatekeeperUser> UserManager;

        public UsersController(UserManager<GatekeeperUser> userManager)
        {
            UserManager = userManager;
        }

        // GET: /Users/5
        [Authorize(AuthenticationSchemes = "token")]
        [HttpGet("{uuid}", Name = "Get")]
        public async Task<IActionResult> Get(string uuid)
        {
            if (uuid == null)
            {
                return NotFound();
            }

            var user = await UserManager.FindByIdAsync(uuid);
            if(user == null)
            {
                return NotFound();
            }

            // We only want a subset of the user fields.
            return Ok(new Dictionary<string, string>() {
                { "id", uuid },
                { "email", user.Email },
                { "name", user.UserName },
            });
        }
    }
}
