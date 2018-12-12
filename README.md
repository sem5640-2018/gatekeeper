# gatekeeper

Gatekeeper is the OAuth2 authorization service for the AberFitness group project.  It will provide:
* Standard OAuth2 grant types
* Authentication via OpenID Connect
* User, Client, and Resource management
* User metadata API

| Branch | Status |
|-|-|
| Development | [![Development Build Status](https://travis-ci.org/sem5640-2018/gatekeeper.svg?branch=development)](https://travis-ci.org/sem5640-2018/gatekeeper) |
| Release | [![Release Build Status](https://travis-ci.org/sem5640-2018/gatekeeper.svg?branch=master)](https://travis-ci.org/sem5640-2018/gatekeeper) |

# Requirements
* [.NET Core SDK 2.1.x][dotnetsdk]
* [LibMan CLI][libmancli]

# Getting Started (Development)
This section lists the basic steps required to correctly run the application in your development environment.  For production environments, read the documentation [here](docs/production-deployment.md).

These instructions are for the .NET CLI which comes as standard with the [.NET Core SDK][dotnetsdk].  You could also use the tools available in Visual Studio.

1. Install the [requirements](#requirements)
1. [Configure your app](docs/runtime-configuration.md)
1. Install the project dependencies using `dotnet restore`
1. Restore the static libraries using `libman restore`
1. Build the solution using with `dotnet build`
1. Run any pending database migrations with `dotnet ef database update --context GatekeeperContext`
   1. On the first run, you'll also need to run the database migrations for IdentityServer4's contexts with `dotnet ef database update --context ConfigurationDbContext` and `dotnet ef database update --context PersistedGrantDbContext`
   1. **HELP** - I got an error starting with "An error occurred while accessing the IWebHost on class 'Program'." - You probably didn't [configure your app](docs/runtime-configuration.md) properly.  Try again.
1. Run the app in development mode using `dotnet run --project Gatekeeper`
1. Run tests using `dotnet test GatekeeperTest`

# Maintainers

* Adam Lancaster (development)
* Dan Monaghan (review)

# More Info

The OAuth2 and OpenID Connect implementation is provided by [IdentityServer4][ids4].

Identity management uses [.NET Core Identity][dotnetidentity].

Configuration of clients and resources, and any other persistent data, will be stored using Entity Framework Core.

Tests are written using [xUnit][xunit].

Runtime configuration will use .NET Core's environment [configuration provider][dotnetconfig].  If configuration files are used, it will be for development and testing environments only.

[dotnetsdk]: https://www.microsoft.com/net/download/dotnet-core/2.1
[libmancli]: https://docs.microsoft.com/en-us/aspnet/core/client-side/libman/libman-cli?view=aspnetcore-2.1
[ids4]: http://docs.identityserver.io
[dotnetidentity]: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.1&tabs=visual-studio
[xunit]: https://xunit.github.io/
[dotnetconfig]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1