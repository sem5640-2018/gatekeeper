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
                services.AddDbContext<GatekeeperContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("GatekeeperContextConnection")));

                services.AddTransient<IEmailSender, EmailSender>();

                services.AddIdentity<GatekeeperUser, IdentityRole>()
                    .AddEntityFrameworkStores<GatekeeperContext>()
                    .AddDefaultTokenProviders();

                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryPersistedGrants()
                    .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())
                    .AddInMemoryApiResources(IdentityConfig.GetApiResources())
                    .AddInMemoryClients(IdentityConfig.GetClients())
                    .AddAspNetIdentity<GatekeeperUser>();
            });
        }
    }
}