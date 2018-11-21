// Adapted from code at http://amilspage.com/signing-certificates-idsv4/

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Gatekeeper.Util
{
    public static class IdentityServerCredentialExtension
    {
        public static IIdentityServerBuilder AddCredentialsForEnvironment(
            this IIdentityServerBuilder builder, IHostingEnvironment environment, IConfigurationSection gatekeeperConfig, ILogger logger)
        {
            logger = logger.ForContext(typeof(IdentityServerCredentialExtension));
            if (!environment.IsDevelopment())
            {
                AddCertificateFromFile(builder, gatekeeperConfig, logger);
            } else
            {
                builder.AddDeveloperSigningCredential();
                logger.Information("Using Developer signing credentials.");
            }

            return builder;
        }

        private static void AddCertificateFromFile(IIdentityServerBuilder builder, IConfigurationSection gatekeeperConfig, ILogger logger)
        {
            var certPath = Path.Combine(gatekeeperConfig.GetValue<string>("CertsPath", "/certs"), "is4cert.pfx");
            var certPassword = gatekeeperConfig.GetValue<string>("TokenCertPassword");

            if (File.Exists(certPath))
            {
                builder.AddSigningCredential(new X509Certificate2(certPath, certPassword));
                logger.Information($"Loaded certificate from {certPath}");
            }
            else
            {
                logger.Fatal($"Certificate not found at {certPath}");
            }
        }
    }
}