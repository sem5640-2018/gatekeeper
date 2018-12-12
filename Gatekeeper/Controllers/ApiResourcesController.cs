using Gatekeeper.Repositories;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gatekeeper.Controllers
{
    public class ApiResourcesController : Controller
    {
        private readonly IApiResourceRepository _repository;

        public ApiResourcesController(IApiResourceRepository repository)
        {
            _repository = repository;
        }

        // GET: ApiResources
        [Authorize("Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllAsync());
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
                await _repository.AddAsync(apiResource);
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

            var apiResource = await _repository.GetByIdAsync(id.Value);
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
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
