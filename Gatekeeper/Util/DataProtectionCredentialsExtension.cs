// Adapted from code at http://amilspage.com/signing-certificates-idsv4/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

                case "Store":
                    AddCertificateFromStore(builder, gatekeeperConfig);
                    break;
            }

            return builder;
        }

        private static void AddCertificateFromStore(IDataProtectionBuilder builder, IConfigurationSection gatekeeperConfig)
        {
            var certThumbprint = gatekeeperConfig.GetValue<string>("DPKCertThumbprint");

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, true);

            if (certificates.Count > 0)
                builder.PersistKeysToFileSystem(new DirectoryInfo(gatekeeperConfig.GetValue<string>("KeysPath")))
                .ProtectKeysWithCertificate(certificates[0]);
            else
                Console.WriteLine("A matching thumbprint couldn't be found in the store");
        }

        private static void AddCertificateFromFile(IDataProtectionBuilder builder, IConfigurationSection gatekeeperConfig)
        {
            var certFilePath = Path.Combine(gatekeeperConfig.GetValue<string>("CertsPath"), "dpkcert.pfx");
            var certFilePassword = gatekeeperConfig.GetValue<string>("DPKCertPassword");

            if (File.Exists(certFilePath))
            {
                builder.PersistKeysToFileSystem(new DirectoryInfo(gatekeeperConfig.GetValue<string>("KeysPath")))
                    .ProtectKeysWithCertificate(new X509Certificate2(certFilePath, certFilePassword));
            }
            else
            {
                Console.WriteLine($"SigninCredentialExtension cannot find cert file {certFilePath}");
            }
        }
    }
}