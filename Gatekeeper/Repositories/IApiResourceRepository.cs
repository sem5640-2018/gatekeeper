using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gatekeeper.Repositories
{
    public interface IApiResourceRepository
    {
        Task<List<ApiResource>> GetAllAsync();

        Task<ApiResource> GetByIdAsync(int id);

        Task AddAsync(ApiResource resource);

        Task DeleteAsync(int id);
    }
}
