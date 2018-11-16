using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Areas.Identity.Services;
using Gatekeeper.Models;
using Gatekeeper.Repositories;
using Gatekeeper.Util;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
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
        private IConfiguration Configuration { get; }
        private IConfigurationSection GatekeeperConfig { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            GatekeeperConfig = Configuration.GetSection("Gatekeeper");
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc()
                .AddRazorPagesOptions(options => {
                    options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/Index", "");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var dbConnectionString = Configuration.GetConnectionString("GatekeeperContextConnection");
            var migrationsAssembly = "Gatekeeper";

            services.AddDbContext<GatekeeperContext>(options =>
                options.UseSqlServer(dbConnectionString));

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddIdentity<GatekeeperUser, IdentityRole>()
                .AddEntityFrameworkStores<GatekeeperContext>()
                .AddDefaultTokenProviders();

            services.AddDataProtection()
                .AddSigningCredentialFromConfig(GatekeeperConfig);

            services.AddIdentityServer(options => {
                    options.UserInteraction.LoginUrl = "/Identity/Account/Login";
                    options.UserInteraction.LogoutUrl = "/Identity/Account/Logout";
                })
                .AddSigningCredentialFromConfig(GatekeeperConfig)
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

            services.AddAuthentication().AddIdentityServerAuthentication("token", options =>
            {
                options.Authority = GatekeeperConfig.GetValue<string>("BaseUrl");
                options.ApiName = "gatekeeper";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", pb => pb.RequireClaim("user_type", "administrator"));
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
            });

            services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ConfigurationDbContext configurationDbContext)
        {
            GatekeeperIdentityResources.PreloadResources(configurationDbContext);

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

            app.UseMvc();
        }
    }
}
