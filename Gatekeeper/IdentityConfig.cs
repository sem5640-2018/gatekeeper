using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gatekeeper
{
    public class IdentityConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // TODO: Real configuration to be stored via EntityFramework Core
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("placeholder_api", "Placeholder API")
            };
        }

        // TODO: Real configuration to be stored via EntityFramework Core
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "placeholder_client",
                    ClientName = "Placeholder Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    ClientSecrets = { new Secret("placeholder_secret".Sha256()) },
                    RedirectUris = { "https://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "placeholder_api"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
    }
}
