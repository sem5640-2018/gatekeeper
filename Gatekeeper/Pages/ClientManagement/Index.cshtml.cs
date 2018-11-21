using Gatekeeper.Repositories;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gatekeeper.Pages.ClientManagement
{
    [Authorize("Administrator")]
    public class IndexModel : PageModel
    {
        private readonly IClientRepository clientRepository;

        public IndexModel(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public IList<Client> Clients { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Clients = await clientRepository.GetAllAsync();

            return Page();
        }
    }
}