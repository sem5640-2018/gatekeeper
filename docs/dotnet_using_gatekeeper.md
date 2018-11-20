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
var appConfiguration = Configuration.GetSection("YourAppName");
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
    options.Authority = appConfiguration.GetValue<string>("GatekeeperUrl");
    options.ClientId = appConfiguration.GetValue<string>("ClientId");
    options.ClientSecret = appConfiguration.GetValue<string>("ClientSecret");
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
var appConfiguration = Configuration.GetSection("YourAppName");
services.AddAuthentication().AddIdentityServerAuthentication("token", options =>
{
    options.Authority = gatekeeperConfig.GetValue<string>("GatekeeperUrl");
    options.ApiName = gatekeeperConfig.GetValue<string>("ApiResourceName");
});
```


Add the following line to your `Configure` method, after the call to `app.UseCookiePolicy();`:

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

Just combined the two examples above.  Chain the call to `AddIdentityServerAuthentication` after the call to `AddOpenIdConnect`.

The only difference here is that when using the `[Authorize]` attribute, you need to specify which challenge scheme to use.  Use `[Authorize(AuthenticationSchemes = "oidc")]` to protect pages where you want users to log in, and `[Authorize(AuthenticationSchemes = "token")]` to protect APIs.

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
    foreach (var claim in User.Claims)
    {
        Console.WriteLine(claim);
    }
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

These steps describe the basic process, however you will probably want to wrap the functionality in an easily reusable component.

Add the `IdentityModel` NuGet package to your application.

Use the `DiscoveryClient` to automatically discover the OAuth endpoints.
In your wrapper class, you should only need to perform this once at startup.
```csharp
var discovery = await DiscoveryClient.GetAsync(appConfig.GetValue<string>("GatekeeperUrl"));
```

Use the `TokenClient` to obtain an access token.
```csharp
var tokenClient = new TokenClient(
    discovery.TokenEndpoint,
    appConfig.GetValue<string>("ClientId"),
    appConfig.GetValue<string>("ClientSecret")
    );
// health_data is used as an example here.  You'll need to know the ApiResource name of the API
// you want to access.
var tokenResponse = await tokenClient.RequestClientCredentialsAsync("health_data");
```

Use the Bearer token you just obtained to make a call to the API:
```csharp
var client = new HttpClient(); // You should probably use an HTTP client factory
client.SetBearerToken(tokenResponse.AccessToken);

var response = await client.GetAsync("https://aberfitness.biz/health_data/whatever-api-youre-calling");
```

---