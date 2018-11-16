using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gatekeeper.Controllers
{
    public class UsersManagementController : Controller
    {
        private UserManager<GatekeeperUser> UserManager;

        public UsersManagementController(UserManager<GatekeeperUser> userManager)
        {
            UserManager = userManager;
        }

        // GET: UsersManagement
        [Authorize("Administrator")]
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        // GET: UsersManagement/Edit/5
        [Authorize("Administrator")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsersManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Administrator")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersManagement/Delete/5
        [Authorize("Administrator")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: UsersManagement/Delete/5
        [Authorize("Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await UserManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}