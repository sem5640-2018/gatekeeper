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

# Quick Start
These instructions assume you're using the .NET CLI which comes as standard with the [.NET Core SDK][dotnetsdk].  You could alternatively use the tools available in Visual Studio.

1. Install the [requirements](#requirements)
1. Install the project dependencies using `dotnet restore`
1. Build the solution using with `dotnet build`
1. Run any pending database migrations using `dotnet ef database update`
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