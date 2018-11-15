using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gatekeeper.Util
{
    public class GatekeeperIdentityResources
    {
        public static void PreloadResources(ConfigurationDbContext context)
        {
            if (!context.IdentityResources.Any())
            {
                foreach (var resource in GetIdentityResources())
                {
                    context.IdentityResources.Add(resource);
                }
                context.SaveChanges();
            }
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                 new IdentityResource()
                 {
                     Enabled = true,
                     Name = "openid",
                     DisplayName = "Your user identifier",
                     Required = true,
                     Emphasize = false,
                     ShowInDiscoveryDocument = true,
                     UserClaims = new List<IdentityClaim>
                     {
                         new IdentityClaim() { Type = "sub" }
                     }
                 },
                 new IdentityResource()
                 {
                     Enabled = true,
                     Name = "profile",
                     DisplayName = "User profile",
                     Description = "Your user profile information (first name, last name, etc.)",
                     Required = false,
                     Emphasize = true,
                     ShowInDiscoveryDocument = true,
                     UserClaims = new List<IdentityClaim>
                     {
                         new IdentityClaim() { Type = "name" },
                         new IdentityClaim() { Type = "family_name" },
                         new IdentityClaim() { Type = "given_name" },
                         new IdentityClaim() { Type = "middle_name" },
                         new IdentityClaim() { Type = "nickname" },
                         new IdentityClaim() { Type = "preferred_username" },
                         new IdentityClaim() { Type = "profile" },
                         new IdentityClaim() { Type = "picture" },
                         new IdentityClaim() { Type = "website" },
                         new IdentityClaim() { Type = "gender" },
                         new IdentityClaim() { Type = "birthdate" },
                         new IdentityClaim() { Type = "zoneinfo" },
                         new IdentityClaim() { Type = "locale" },
                         new IdentityClaim() { Type = "updated_at" },
                         new IdentityClaim() { Type = "user_type" }
                     }
                 }
            };
        }
    }
}
