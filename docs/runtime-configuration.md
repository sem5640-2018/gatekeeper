# Runtime Configuration

The prefered way to set configuration is using environment variables.

In **production**, these should actually be set in the environment.  In **development**, your IDE may be able to help you set them.  For example, in Visual Studio, you can set environment variables like this:

![Setting environment variables in Visual Studio][vs_envvars]

**Note:** As per the [.NET Configuration documentation][dotnetconfig], keys are case-insensitive whilst values are not.

[dotnetconfig]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1
[vs_envvars]: ./vs_envvars.png

## Required Keys (All Environments)

| Environment Variable | Default | Description |
|-|-|-|
| ASPNETCORE_ENVIRONMENT | Production | Runtime environment, should be 'Development', 'Staging', or 'Production'. |
| ConnectionStrings__GatekeeperContext | N/A | MariaDB connection string. |
| Gatekeeper__OAuthAuthorityUrl | N/A | HTTPS url of the OAuth authority.  This will actually be the URL of gatekeeper itself, as it also provides APIs protected by itself. |
| Gatekeeper__ApiResourceName | gatekeeper | The name of this API resource. |

## Required Keys (Production + Staging Environments)
In addition to the above keys, you will also require:

| Environment Variable | Default | Description |
|-|-|-|
| Gatekeeper__ReverseProxyHostname | http://nginx | The internal docker hostname of the reverse proxy being used. |
| Gatekeeper__PathBase | /gatekeeper | The pathbase (name of the directory) that Gatekeeper is being served from. |
| Gatekeeper__KeysPath | /keys | Path to the directory where .NET should persist auto-managed data protection keys. |
| Gatekeeper__CertsPath |  /certs | Path to the directory containing application certificates (is4cert.pfx and dpkcert.pfx). |
| Gatekeeper__TokenCertPassword | N/A | The password used to protect your token signing certificate (is4cert.pfx). |
| Gatekeeper__DPKCertPassword | N/A | The password used to protect your .NET data protection key certificate (dpkcert.pfx). |