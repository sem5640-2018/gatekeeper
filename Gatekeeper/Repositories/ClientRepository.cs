using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;

namespace Gatekeeper.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ConfigurationDbContext _context;

        public ClientRepository(ConfigurationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Client client)
        {
            _context.Add(client);
            return _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        public Task<List<Client>> GetAllAsync()
        {
            return _context.Clients.ToListAsync();
        }

        public Task<Client> GetByIdAsync(int id)
        {
            return _context.Clients.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
