// Adapted from code at http://amilspage.com/signing-certificates-idsv4/

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Gatekeeper.Util
{
    public static class DataProtectionCredentialExtension
    {
        public static IDataProtectionBuilder AddCredentialsForEnvironment(
            this IDataProtectionBuilder builder,
            IHostingEnvironment environment,
            IConfigurationSection gatekeeperConfig,
            ILogger logger)
        {
            logger = logger.ForContext(typeof(DataProtectionCredentialExtension));
            if (!environment.IsDevelopment())
            {
                AddCertificateFromFile(builder, gatekeeperConfig, logger);
            }
            else
            {
                logger.Information("Data Protection Keys using Developer mode.");
            }

            return builder;
        }

        private static void AddCertificateFromFile(IDataProtectionBuilder builder, IConfigurationSection gatekeeperConfig, ILogger logger)
        {
            var certPath = Path.Combine(gatekeeperConfig.GetValue<string>("CertsPath", "/certs"), "dpkcert.pfx");
            var certPassword = gatekeeperConfig.GetValue<string>("DPKCertPassword");
            var keysPath = gatekeeperConfig.GetValue<string>("KeysPath", "/keys");

            if (File.Exists(certPath))
            {
                builder.PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                    .ProtectKeysWithCertificate(new X509Certificate2(certPath, certPassword));
                logger.Information($"Loaded certificate from {certPath}");
            }
            else
            {
                logger.Fatal($"Certificate not found at {certPath}");
            }
        }
    }
}