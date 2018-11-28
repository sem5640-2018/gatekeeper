using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gatekeeper.Services
{
    public interface ICommsApiClient
    {
        Task<HttpResponseMessage> PostAsync(string path, object payload);
    }

    public class CommsApiClient : ICommsApiClient
    {
        private readonly HttpClient client;
        private readonly IConfigurationSection appConfig;
        private readonly DiscoveryCache discoveryCache;
        private readonly ILogger logger;

        public CommsApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<CommsApiClient> log)
        {
            appConfig = configuration.GetSection("Gatekeeper");
            discoveryCache = new DiscoveryCache(appConfig.GetValue<string>("GatekeeperUrl"));
            client = httpClientFactory.CreateClient("comms");
            logger = log;
        }

        private async Task<string> GetTokenAsync()
        {
            var discovery = await discoveryCache.GetAsync();
            if (discovery.IsError)
            {
                logger.LogError(discovery.Error);
                throw new CommsApiClientException("Couldn't read discovery document.");
            }

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = discovery.TokenEndpoint,
                ClientId = appConfig.GetValue<string>("ClientId"),
                ClientSecret = appConfig.GetValue<string>("ClientSecret"),
                Scope = "comms"
            };
            var response = await client.RequestClientCredentialsTokenAsync(tokenRequest);
            if (response.IsError)
            {
                logger.LogError(response.Error);
                throw new CommsApiClientException("Couldn't retrieve access token.");
            }
            return response.AccessToken;
        }

        public async Task<HttpResponseMessage> PostAsync(string uri, object payload)
        {
            client.SetBearerToken(await GetTokenAsync());
            return await client.PostAsJsonAsync(uri, payload);
        }
    }

    public class CommsApiClientException : Exception
    {
        public CommsApiClientException(string message) : base(message)
        {
        }
    }
}
