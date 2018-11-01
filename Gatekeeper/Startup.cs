using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Areas.Identity.Services;
using Gatekeeper.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gatekeeper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var dbConnectionString = Configuration.GetConnectionString("GatekeeperContextConnection");
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
