using Gatekeeper.Repositories;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Gatekeeper.Pages.ClientManagement
{
    [Authorize("Administrator")]
    public class CreateModel : PageModel
    {
        private readonly IClientRepository clientRepository;

        public CreateModel(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public List<SelectListItem> AvailableGrantTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "hybrid", Text = "Hybrid" },
            new SelectListItem { Value = "client_credentials", Text = "Client Credentials" },
            new SelectListItem { Value = "authorization_code", Text = "Code" },
            new SelectListItem { Value = "implicit", Text = "Implicit" }
        };


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Client ID")]
            public string ClientId { get; set; }

            [Required]
            [Display(Name = "Client Name")]
            public string ClientName { get; set; }

            [Required]
            [Display(Name = "Grant Types")]
            public List<string> GrantTypes { get; set; }

            [Required]
            [Display(Name = "Redirect URIs", Description = "Space-seperated list of valid Redirect URIs.")]
            public string RedirectUri { get; set; }

            [Required]
            [Display(Name = "Scopes", Description = "Space-seperated list of scopes this client can access.")]
            public string Scopes { get; set; }

            [Required]
            [DataType("password")]
            [Display(Name = "Client Secret")]
            public string ClientSecret { get; set; }

        }

        public IActionResult OnGet()
        {
            Input = new InputModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var client = new Client()
            {
                ClientId = Input.ClientId,
                ClientName = Input.ClientName,
                AllowOfflineAccess = true,
                Enabled = true,
                AllowedGrantTypes = Input.GrantTypes,
                RedirectUris = Input.RedirectUri.Split(" "),
                AllowedScopes = Input.Scopes.Split(" "),
                RequireConsent = false,
                ClientSecrets =
                {
                    new Secret(Input.ClientSecret.Sha256())
                }
            };

            await clientRepository.AddAsync(client.ToEntity());

            return RedirectToPage("Index");
        }
    }
}