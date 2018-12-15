using AberFitnessAuditLogger;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gatekeeper.Pages.UserManagement
{
    [Authorize("Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly IAuditLogger auditLogger;

        public DeleteModel(IUserRepository userRepository, IAuditLogger auditLogger)
        {
            this.userRepository = userRepository;
            this.auditLogger = auditLogger;
        }

        public GatekeeperUser UserToDelete { get; set; }

        private string CurrentUserId()
        {
            try
            {
                return User.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;
            }
            catch (NullReferenceException)
            {
                return "Unknown";
            }
        }

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
            await auditLogger.log(id, $"Deleted by {CurrentUserId()}");

            return RedirectToPage("Index");
        }
    }
}