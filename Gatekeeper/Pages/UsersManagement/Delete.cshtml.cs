using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gatekeeper.Pages.UsersManagement
{
    [Authorize("Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<GatekeeperUser> _userManager;

        public DeleteModel(UserManager<GatekeeperUser> userManager)
        {
            _userManager = userManager;
        }

        public GatekeeperUser UserToDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            UserToDelete = user;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return RedirectToPage("Index");
        }
    }
}