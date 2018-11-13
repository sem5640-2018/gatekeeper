using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Gatekeeper.Controllers
{
    public class ApiResourcesController : Controller
    {
        private readonly ConfigurationDbContext _context;

        public ApiResourcesController(ConfigurationDbContext context)
        {
            _context = context;
        }

        // GET: ApiResources
        [Authorize("Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApiResources.ToListAsync());
        }

        // GET: ApiResources/Create
        [Authorize("Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApiResources/Create
        [Authorize("Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,DisplayName,Description")] ApiResource apiResource)
        {
            if (ModelState.IsValid)
            {
                apiResource.Enabled = true;
                apiResource.Scopes = new List<ApiScope> { new ApiScope() { Name = apiResource.Name, DisplayName = apiResource.DisplayName } };
                _context.Add(apiResource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(apiResource);
        }

        // GET: ApiResources/Delete/5
        [Authorize("Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiResource = await _context.ApiResources
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apiResource == null)
            {
                return NotFound();
            }

            return View(apiResource);
        }

        // POST: ApiResources/Delete/5
        [Authorize("Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apiResource = await _context.ApiResources.FindAsync(id);
            _context.ApiResources.Remove(apiResource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApiResourceExists(int id)
        {
            return _context.ApiResources.Any(e => e.Id == id);
        }
    }
}
