# Runtime Configuration

In **development** environments, you can set configuration either by using environment variables, or by using `appsettings.Development.json`.

In **production** environments, you must set configuration using environment variables.

You should read and understand [Configuration in ASP.NET Core][dotnetconfig]

[dotnetconfig]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1

## Required Keys

| Environment Variable | appsettings.Development key | Description |
|-|-|-|
| ConnectionStrings_GatekeeperContextConnection |  ConnectionStrings.GatekeeperContextConnection | SQL connection string.
