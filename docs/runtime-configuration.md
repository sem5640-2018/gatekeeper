# Runtime Configuration

In **development** environments, you can set configuration either by using environment variables, or by using `appsettings.Development.json`.

In **production** environments, you must set configuration using environment variables.

You should read and understand [Configuration in ASP.NET Core][dotnetconfig]

**Note:** Keys are case-insensitive, as per the documentation.

[dotnetconfig]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1

## Required Keys

| Environment Variable | appsettings.Development key | Description |
|-|-|-|
| ASPNETCORE_ENVIRONMENT | N/A | Runtime environment, should be 'Development', 'Staging', or 'Production'.  Defaults to 'Production'|
| ConnectionStrings_GatekeeperContextConnection |  ConnectionStrings.GatekeeperContextConnection | MSSQL connection string.
| Gatekeeper__TokenCertPassword | Gatekeeper.TokenCertPassword | The password used to protect your token signing certificate. |
| Gatekeeper__CertsPath | Gatekeeper.CertsPath | Path to the directory containing application certificates, including both the token signing certificate and certificates self-managed by .NET |
| Gatekeeper__KeysPath | Gatekeeper.KeysPath | Path to the directory where .NET should persist its data protection keys. |