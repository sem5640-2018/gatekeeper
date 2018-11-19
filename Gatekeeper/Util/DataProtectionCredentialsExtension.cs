// Adapted from code at http://amilspage.com/signing-certificates-idsv4/

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;

namespace Gatekeeper.Util
{
    public static class DataProtectionCredentialExtension
    {
        public static IDataProtectionBuilder AddCredentialsForEnvironment(
            this IDataProtectionBuilder builder, IHostingEnvironment environment, IConfigurationSection gatekeeperConfig)
        {

            if(!environment.IsDevelopment())
            {
                AddCertificateFromFile(builder, gatekeeperConfig);
            }

            return builder;
        }

        private static void AddCertificateFromFile(IDataProtectionBuilder builder, IConfigurationSection gatekeeperConfig)
        {
            var certPath = Path.Combine(gatekeeperConfig.GetValue<string>("CertsPath", "/certs"), "dpkcert.pfx");
            var certPassword = gatekeeperConfig.GetValue<string>("DPKCertPassword");
            var keysPath = gatekeeperConfig.GetValue<string>("KeysPath", "/keys");

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