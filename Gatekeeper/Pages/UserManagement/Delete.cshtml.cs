using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gatekeeper.Pages.UserManagement
{
    [Authorize("Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public DeleteModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public GatekeeperUser UserToDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            UserToDelete = user;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await userRepository.DeleteAsync(id);

            return RedirectToPage("Index");
        }
    }
}