// Adapted from code at http://amilspage.com/signing-certificates-idsv4/

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Gatekeeper.Util
{
    public static class DataProtectionCredentialExtension
    {
        public static IDataProtectionBuilder AddSigningCredentialFromConfig(
            this IDataProtectionBuilder builder, IConfigurationSection gatekeeperConfig)
        {
            string certStorageType = gatekeeperConfig.GetValue<string>("CertStorageType");

            switch (certStorageType)
            {
                case "File":
                    AddCertificateFromFile(builder, gatekeeperConfig);
                    break;
                default:
                    // Do nothing, .NET will manage developer keys for us.
                    break;
            }

            return builder;
        }

        private static void AddCertificateFromFile(IDataProtectionBuilder builder, IConfigurationSection gatekeeperConfig)
        {
            var certPath = Path.Combine(gatekeeperConfig.GetValue<string>("CertsPath"), "dpkcert.pfx");
            var certPassword = gatekeeperConfig.GetValue<string>("DPKCertPassword");
            var keysPath = gatekeeperConfig.GetValue<string>("KeysPath");

            if (File.Exists(certPath))
            {
                builder.PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                    .ProtectKeysWithCertificate(new X509Certificate2(certPath, certPassword));
            }
            else
            {
                Console.WriteLine($"SigninCredentialExtension cannot find cert file {certPath}");
            }
        }
    }
}