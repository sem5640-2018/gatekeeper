﻿using AberFitnessAuditLogger;
using Gatekeeper.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gatekeeper.Pages.UserManagement
{
    [Authorize("Administrator")]
    public class EditModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly IAuditLogger auditLogger;

        public EditModel(IUserRepository userRepository, IAuditLogger auditLogger)
        {
            this.userRepository = userRepository;
            this.auditLogger = auditLogger;
        }

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

        public List<SelectListItem> MemberTypes { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "member", Text = "Member" },
            new SelectListItem { Value = "coordinator", Text = "Coordinator" },
            new SelectListItem { Value = "administrator", Text = "Administrator" },
        };

        [TempData]
        public string StatusMessage { get; set; }

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
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var claims = await userRepository.GetClaimAsync(user, "user_type");
            var userType = claims.Count(c => c.Type == "user_type") == 1 ? claims.Single(c => c.Type == "user_type") : new Claim("user_type", "member");

            Input = new InputModel
            {
                Email = user.Email,
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

            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var claims = await userRepository.GetClaimAsync(user, "user_type");
            var userType = claims.Count(c => c.Type == "user_type") == 1 ? claims.Single(c => c.Type == "user_type") : new Claim("user_type", "member");

            user.Email = Input.Email;
            user.UserName = Input.Email;

            await userRepository.UpdateAsync(user);
            await userRepository.AddOrReplaceClaimAsync(user, new Claim("user_type", Input.UserType));
            await auditLogger.log(id, $"Edited by {CurrentUserId()}");

            StatusMessage = "User has been updated";

            return RedirectToPage();
        }
    }
}