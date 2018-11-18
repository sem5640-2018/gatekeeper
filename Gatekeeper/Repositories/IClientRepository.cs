using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gatekeeper.Repositories
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllAsync();

        Task<Client> GetByIdAsync(int id);

        Task AddAsync(Client client);

        Task DeleteAsync(int id);
    }
}
