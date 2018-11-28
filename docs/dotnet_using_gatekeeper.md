# .NET Core - Using Gatekeeper

## Get your application Client ID/Secret

You need a Client ID and Secret to use Gatekeeper.  These will be different for each development, staging, and production environment.  Join `#dev-gatekeeper` in Slack to get a client ID/secret for development.  For staging/production, our docker-compose file will take care of setting these as environment variables inside the container running your app.

Your application should load the client ID and secret from environment variables, as shown later.  You can set environment variables for your development environment in Visual Studio by right clicking your project, opening Properties -> Debug, and entering them in the Environment Variables section.  In order for .NET Core to read these values automatically, you should prefix them with your application name and two underscores.  For example, if your application is called HealthData, your environment variables should start with `HealthData__`.

![Setting environment variables in Visual Studio][VS_EnvironmentVariables]

[VS_EnvironmentVariables]: ./vs_envvars.png

---

## My app has a UI and I want to use Gatekeeper to log in
Add the following code to your `ConfigureServices` method:

```csharp
var appConfig = Configuration.GetSection("YourAppName");
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.SignInScheme = "Cookies";
    options.Authority = appConfig.GetValue<string>("GatekeeperUrl");
    options.ClientId = appConfig.GetValue<string>("ClientId");
    options.ClientSecret = appConfig.GetValue<string>("ClientSecret");
    options.ResponseType = "code id_token";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("profile");
    options.Scope.Add("offline_access");
    options.ClaimActions.MapJsonKey("locale", "locale");
    options.ClaimActions.MapJsonKey("user_type", "user_type");
});
```

Add the following line to your `Configure` method, after the call to `app.UseCookiePolicy();`:

```csharp
app.UseAuthentication();
```

To reuqire that a page in your app requires the user to be logged in, use the `[Authorize]` attribute on the controller or action.
```csharp
[Authorize]
public IActionResult Index()
{
    return View();
}
```

### I want to restrict certain routes/pages to coordinators/administrators

Add authorization policies in your `ConfigureServices` method:

```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", pb => pb.RequireClaim("user_type", "administrator"));

    // Coordinator policy allows both Coordinators and Administrators
    options.AddPolicy("Coordinator", pb => pb.RequireClaim("user_type", new[] { "administrator", "coordinator" }));
});
```

On actions you want to protect, use the `[Authorize]` attribute with a policy
```csharp
[Authorize("Administrator")]
public IActionResult Index()
{
    return View();
}
```

---

## My app has an API I want to protect

Go into `#dev-gatekeeper` on slack and ask that an API resource be defined for your application.  You'll be given an `ApiResourceName` to use.  Set this as an environment variable, as described above.

Add the `IdentityServer4.AccessTokenValidation` NuGet package to your project.

Add the following code to your `ConfigureServices` method:

```csharp
var appConfig = Configuration.GetSection("YourAppName");
services.AddAuthentication(options =>
{
    options.DefaultScheme = "Bearer";
})
.AddIdentityServerAuthentication("Bearer", options =>
{
    options.Authority = appConfig.GetValue<string>("GatekeeperUrl");
    options.ApiName = appConfig.GetValue<string>("ApiResourceName");
});
```


Add the following line to your `Configure` method, after the call to `app.UseHttpsRedirection();`:

```csharp
app.UseAuthentication();
```

To define that an API route needs authorization, use the `[Authorize]` attribute on the controller or action.
```csharp
[Authorize]
public async Task<IActionResult> Get(string uuid)
{
    return Ok();
}
```

---

## My app has both a UI, and an API I want to protect

Combine the two examples above.  Chain the call to `AddIdentityServerAuthentication` after the call to `AddOpenIdConnect`.

The only difference here is that when using the `[Authorize]` attribute, you need to specify which challenge scheme to use.  Use `[Authorize(AuthenticationSchemes = "oidc")]` to protect pages where you want users to log in, and `[Authorize(AuthenticationSchemes = "Bearer")]` to protect APIs.

If you're also using Administrator/Coordinator policies, you can combine them:
```csharp
[Authorize(AuthenticationSchemes = "oidc", Policy = "Administrator")]
```

---

## I want to get information about the currently logged in User

### Controllers
In any controller where you've used the `[Authorize]` attribute, the `User` object will be available, inherited from the base controller class.

```csharp
[Authorize]
public IActionResult Index()
{
    var userClaims = User.Claims;
    // do something with the user claims
    return View();
}
```

**Note**: The "User" might be an actual human user, or it might be another service within the application.  Therefore, the attributes available on the `User` object will vary.

### Views

You can access the current user in a view simply by calling the `User` object.

```html
@using Microsoft.AspNetCore.Authentication
@{
    ViewData["Title"] = "Home Page";
}

<div class="row">
    <div class="col-md-12">
        <h2>User Claims</h2>
        <ul>
            @foreach (var claim in User.Claims)
            {
                <li><strong>@claim.Type:</strong> @claim.Value</li>
            }
        </ul>
    </div>
</div>

```

---

## I need to make requests to another service's protected API

Essentially you need to obtain an AccessToken using your Client ID and Secret, and then use that access token when
making requests to the API.

Add the `IdentityModel` NuGet package to your application.

An example of a class which manages getting tokens for you is below.  You could extend this example by adding methods such as PostAsync() to the interface/class.

```csharp
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace YourApp.Services
{
    public interface IApiClient
    {
        Task<HttpResponseMessage> GetAsync(string path);
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient client;
        private readonly IConfigurationSection appConfig;
        private readonly DiscoveryCache discoveryCache;
        private readonly ILogger logger;

        public ApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ApiClient> log)
        {
            appConfig = configuration.GetSection("YourAppName");
            discoveryCache = new DiscoveryCache(appConfig.GetValue<string>("GatekeeperUrl"));
            client = httpClientFactory.CreateClient("yourNamedHttpClient");
            logger = log;
        }

        private async Task<string> GetTokenAsync()
        {
            var discovery = await discoveryCache.GetAsync();
            if (discovery.IsError)
            {
                logger.LogError(discovery.Error);
                throw new ApiClientException("Couldn't read discovery document.");
            }

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = discovery.TokenEndpoint,
                ClientId = appConfig.GetValue<string>("ClientId"),
                ClientSecret = appConfig.GetValue<string>("ClientSecret"),

                // The ApiResourceName of the resources you want to access.
                // Other valid values might be `comms`, `health_data_repository`, etc.
                // Ask in #dev-gatekeeper for help
                Scope = "gatekeeper"
            };
            var response = await client.RequestClientCredentialsTokenAsync(tokenRequest);
            if(response.IsError)
            {
                logger.LogError(response.Error);
                throw new ApiClientException("Couldn't retrieve access token.");
            }
            return response.AccessToken;
        }

        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            client.SetBearerToken(await GetTokenAsync());
            return await client.GetAsync(uri);
        }
    }

    public class ApiClientException : Exception
    {
        public ApiClientException(string message) : base(message)
        {
        }
    }
}
```

In your `Startup.cs`, add it as a singleton:

```csharp
services.AddHttpClient("yourNamedHttpClient", client => {
    client.SomeOptions = "whatever httpclient opens you want to set go here"
});
services.AddSingleton<IApiClient, ApiClient>();
```

Use it as follows:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace YourApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        private readonly IApiClient apiClient;

        public ExampleController(IApiClient client) {
            apiClient = client;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var someData = await apiClient.GetAsync("https://aberfitness.biz/some_service/whatever_api_youre_calling");
            // do something with someData
            return Ok();
        }
    }
}
```

---