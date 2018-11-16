using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gatekeeper.Pages.UserManagement
{
    [Authorize("Administrator")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<GatekeeperUser> _userManager;

        public IndexModel(UserManager<GatekeeperUser> userManager)
        {
            _userManager = userManager;
        }

        public List<GatekeeperUser> Users { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Users = await _userManager.Users.ToListAsync();

            return Page();
        }
    }
}