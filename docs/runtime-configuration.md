# Runtime Configuration

The prefered way to set configuration is using environment variables.

In **production**, these should actually be set in the environment.  In **development**, your IDE may be able to help you set them.  For example, in Visual Studio, you can set environment variables like this:

![Setting environment variables in Visual Studio][vs_envvars]

**Note:** As per the [.NET Configuration documentation][dotnetconfig], keys are case-insensitive whilst values are not.

[dotnetconfig]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.1
[vs_envvars]: ./vs_envvars.png

## Required Keys

| Environment Variable | Description |
|-|-|
| ASPNETCORE_ENVIRONMENT | Runtime environment, should be 'Development', 'Staging', or 'Production'.  Defaults to 'Production'|
| ConnectionStrings__GatekeeperContextConnection | MSSQL connection string. |
| Gatekeeper__KeysPath | Path to the directory where .NET should persist auto-managed data protection keys. |
| Gatekeeper__CertStorageType | Defines where the application should load certificates from.  Options are 'File', or 'Development'. See [certificates documentation](certificates.md). |
| Gatekeeper__BaseUrl | The full HTTPS url of this gatekeeper instance.  Required as the application has APIs protected by the very OAuth authority it provides. |

## Optional Keys
| Environment Variable | Description |
|-|-|
| Gatekeeper__CertsPath | **Must be set if using 'File' CertStorageType.** Path to the directory containing application certificates (is4cert.pfx and dpkcert.pfx). |
| Gatekeeper__TokenCertPassword | **Must be set if using 'File' CertStorageType.** The password used to protect your token signing certificate (is4cert.pfx). |
| Gatekeeper__DPKCertPassword |**Must be set if using 'File' CertStorageType.** The password used to protect your .NET data protection key certificate (dpkcert.pfx). |