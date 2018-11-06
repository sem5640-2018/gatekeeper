# .NET MVC - Using Gatekeeper

Adding OpenID to your application is relatively straightforward in .NET Core.  A basic example application is included in the Gatekeeper repository.

**This documentation will be further updated to cover single-sign-out, authorization policies (i.e. restricting access to administrators), and making requests to other services using the User's access token.**

## Claims
Users have a number of claims.  Claims are simply key/value pairs which contain a piece of information about that user.  The claims we're most interested in are:

| Key | Description |
|-|-|
| `sub` | Subject Identifier, AKA the Unique ID of a user. |
| `locale` | The user's preferred locale for localisation. |
| `user_type` | One of `member`, `coordinator`, or `administrator` |
| `name` | The user's name.  Possibly an email address if no name has been set.


## Startup.cs

Add the following code to your `ConfigureServices` method:

```csharp
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
    options.Authority = "https://the_gatekeeper_url";
    options.ClientId = "your_client_id";
    options.ClientSecret = "your_client_secret";
    options.ResponseType = "code id_token";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("profile");
    options.Scope.Add("offline_access");
    options.ClaimActions.MapJsonKey("locale", "locale");
    options.ClaimActions.MapJsonKey("user_type", "user_type");
});
```

The example application from the repository shows an example of loading the URL, Client ID, and Client Secret from the environment rather than using hardcoded strings.


Add the following line to your `Configure` method, after the call to `app.UseCookiePolicy();`:

```csharp
app.UseAuthentication();
```

## Controllers

Basic protection of a route (requiring at least a logged-in user) in a controller requires adding the `[Authorize]` attribute.

You can also access the current user simply by calling the `User` class.

```csharp
using Microsoft.AspNetCore.Authorization;
//class/namespace definition not shown

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

## Views

You can access the current user in a view simply by calling the `User` class.

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

![Example of user claims in a view][example_claims]

[example_claims]: ./example_claims.PNG