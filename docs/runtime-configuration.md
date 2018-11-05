# Runtime Configuration

In **development** environments, you can set configuration either by using environment variables, or by using `appsettings.Development.json`.

In **production** environments, you must set configuration using environment variables.

You should read and understand [Configuration in ASP.NET Core][dotnetconfig]

**Note:** Keys are case-insensitive whilst values are not, as per the documentation.

[dotnetconfig]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1

## Required Keys

| Environment Variable | appsettings.Development key | Description |
|-|-|-|
| ASPNETCORE_ENVIRONMENT | N/A | Runtime environment, should be 'Development', 'Staging', or 'Production'.  Defaults to 'Production'|
| ConnectionStrings_GatekeeperContextConnection |  ConnectionStrings.GatekeeperContextConnection | MSSQL connection string. |
| Gatekeeper__TokenCertPassword | Gatekeeper.TokenCertPassword | The password used to protect your token signing certificate (is4cert.pfx). |
| Gatekeeper__DPKCertPassword | Gatekeeper.DPKCertPassword | The password used to protect your .NET data protection key certificate (dpkcert.pfx). |
| Gatekeeper__CertsPath | Gatekeeper.CertsPath | Path to the directory containing application certificates (is4cert.pfx and dpkcert.pfx). |
| Gatekeeper__KeysPath | Gatekeeper.KeysPath | Path to the directory where .NET should persist auto-managed data protection keys. |