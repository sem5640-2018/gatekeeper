using System;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Areas.Identity.Services;
using Gatekeeper.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Gatekeeper.Areas.Identity.IdentityHostingStartup))]
namespace Gatekeeper.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            

            builder.ConfigureServices((context, services) => {
                var dbConnectionString = context.Configuration.GetConnectionString("GatekeeperContextConnection");
                var migrationsAssembly = "Gatekeeper";

                services.AddDbContext<GatekeeperContext>(options =>
                    options.UseSqlServer(dbConnectionString));

                services.AddTransient<IEmailSender, EmailSender>();

                services.AddIdentity<GatekeeperUser, IdentityRole>()
                    .AddEntityFrameworkStores<GatekeeperContext>()
                    .AddDefaultTokenProviders();

                services.AddIdentityServer(options => options.UserInteraction.LoginUrl = "/Identity/Account/Login")
                    .AddDeveloperSigningCredential()
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = dbBuilder => dbBuilder.UseSqlServer(dbConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = dbBuilder => dbBuilder.UseSqlServer(dbConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        options.EnableTokenCleanup = true;
                    })
                    .AddAspNetIdentity<GatekeeperUser>();
            });
        }
    }
}