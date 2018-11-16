using Gatekeeper.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gatekeeper.Repositories
{
    public interface IUserRepository
    {
        Task<IList<GatekeeperUser>> GetAllAsync();

        Task<GatekeeperUser> GetByIdAsync(string id);

        Task UpdateAsync(GatekeeperUser user);

        Task DeleteAsync(string id);

        Task<IEnumerable<Claim>> GetClaimAsync(GatekeeperUser user, string claimType);

        Task AddOrReplaceClaimAsync(GatekeeperUser user, Claim claim);
    }
}
