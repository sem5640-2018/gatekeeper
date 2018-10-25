using System;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
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

                services.AddDefaultIdentity<GatekeeperUser>()
                    .AddEntityFrameworkStores<GatekeeperContext>();
            });
        }
    }
}