using Gatekeeper.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gatekeeper.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<GatekeeperUser> _userManager;

        public UserRepository(UserManager<GatekeeperUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<IList<GatekeeperUser>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IList<GatekeeperUser>> GetBatchAsync(string[] ids)
        {
            return await _userManager.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        }

        public async Task<GatekeeperUser> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Claim>> GetClaimAsync(GatekeeperUser user, string claimType)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(c => c.Type == claimType);
        }

        public async Task AddOrReplaceClaimAsync(GatekeeperUser user, Claim claim)
        {
            var claimsToRemove = await GetClaimAsync(user, claim.Type);
            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
            await _userManager.AddClaimAsync(user, claim);
        }

        public async Task UpdateAsync(GatekeeperUser user)
        {
            await _userManager.UpdateAsync(user);
        }
    }
}
