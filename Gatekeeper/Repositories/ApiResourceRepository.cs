using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gatekeeper.Repositories
{
    public class ApiResourceRepository : IApiResourceRepository
    {
        private readonly ConfigurationDbContext _context;

        public ApiResourceRepository(ConfigurationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(ApiResource resource)
        {
            _context.Add(resource);
            return _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var resource = await _context.ApiResources.FindAsync(id);
            _context.ApiResources.Remove(resource);
            await _context.SaveChangesAsync();
        }

        public Task<List<ApiResource>> GetAllAsync()
        {
            return _context.ApiResources.ToListAsync();
        }

        public Task<ApiResource> GetByIdAsync(int id)
        {
            return _context.ApiResources.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
