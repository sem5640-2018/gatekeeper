using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gatekeeper.Pages.UserManagement
{
    [Authorize("Administrator")]
    public class IndexModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public IndexModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IList<GatekeeperUser> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await userRepository.GetAllAsync();

            return Page();
        }
    }
}