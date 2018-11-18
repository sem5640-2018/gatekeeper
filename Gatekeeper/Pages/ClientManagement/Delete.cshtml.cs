using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatekeeper.Repositories;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gatekeeper.Pages.ClientManagement
{
    [Authorize("Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly IClientRepository clientRepository;

        public DeleteModel(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public Client ClientToDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = await clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            ClientToDelete = client;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var client = await clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            await clientRepository.DeleteAsync(id);

            return RedirectToPage("Index");
        }
    }
}