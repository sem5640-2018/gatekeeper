using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gatekeeper.Pages.UserManagement
{
    [Authorize("Administrator")]
    public class EditModel : PageModel
    {
        private readonly UserManager<GatekeeperUser> _userManager;

        public EditModel(UserManager<GatekeeperUser> userManager)
        {
            _userManager = userManager;
        }

        public List<SelectListItem> MemberTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "member", Text = "Member" },
            new SelectListItem { Value = "coordinator", Text = "Coordinator" },
            new SelectListItem { Value = "administrator", Text = "Administrator" },
        };

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [RegularExpression("member|administrator|coordinator")]
            public string UserType { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var email = await _userManager.GetEmailAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var userType = claims.Count(c => c.Type == "user_type") == 1 ? claims.Single(c => c.Type == "user_type") : new Claim("user_type", "member");

            Input = new InputModel
            {
                Email = email,
                UserType = userType.Value
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var userType = claims.Count(c => c.Type == "user_type") == 1 ? claims.Single(c => c.Type == "user_type") : new Claim("user_type", "member");

            await _userManager.SetUserNameAsync(user, Input.Email);
            await _userManager.SetEmailAsync(user, Input.Email);
            await _userManager.RemoveClaimAsync(user, userType);
            await _userManager.AddClaimAsync(user, new Claim("user_type", Input.UserType));

            return RedirectToPage();
        }
    }
}