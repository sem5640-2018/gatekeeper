using Gatekeeper.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gatekeeper.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository Repository;

        public UsersController(IUserRepository repository)
        {
            Repository = repository;
        }

        // GET: /Users/some-guid-string-abc
        [Authorize(AuthenticationSchemes = "token")]
        [HttpGet("{uuid}", Name = "Get")]
        public async Task<IActionResult> Get(string uuid)
        {
            if (uuid == null)
            {
                return NotFound();
            }

            var user = await Repository.GetByIdAsync(uuid);
            if (user == null)
            {
                return NotFound();
            }

            // We only want a subset of the user fields.
            return Ok(new Dictionary<string, string>() {
                { "id", user.Id },
                { "email", user.Email },
                { "name", user.UserName },
            });
        }

        // POST: /Users/Batch/
        [Authorize(AuthenticationSchemes = "token")]
        [HttpPost("Batch", Name = "GetBatch")]
        public async Task<IActionResult> GetBatch([FromBody] string[] uuids)
        {
            var users = await Repository.GetBatchAsync(uuids);
            var sanitizedUsers = new List<object>();
            foreach (var user in users)
            {
                sanitizedUsers.Add(new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.UserName
                });
            }

            return Ok(sanitizedUsers);
        }
    }
}
