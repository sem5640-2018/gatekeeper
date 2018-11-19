using System.Collections.Generic;
using System.Linq;
using System.Net;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Areas.Identity.Services;
using Gatekeeper.Models;
using Gatekeeper.Repositories;
using Gatekeeper.Util;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IConfiguration config;
        private readonly IConfigurationSection gatekeeperConfig;
        private readonly IHostingEnvironment environment;

        public Startup(IConfiguration config, IHostingEnvironment environment)
        {
            this.environment = environment;
            this.config = config;
            gatekeeperConfig = this.config.GetSection("Gatekeeper");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnectionString = config.GetConnectionString("GatekeeperContext");
            var migrationsAssembly = "Gatekeeper";

            services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false; // We only use essential cookies
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .AddRazorPagesOptions(options => {
                    options.Conventions.AddAreaPageRoute("Identity", "/Account/Manage/Index", "");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<GatekeeperContext>(options => options.UseMySql(dbConnectionString));

            services.AddIdentity<GatekeeperUser, IdentityRole>()
                .AddEntityFrameworkStores<GatekeeperContext>()
                .AddDefaultTokenProviders();

            services.AddDataProtection().AddCredentialsForEnvironment(environment, gatekeeperConfig);

            services.AddIdentityServer(options => {
                options.UserInteraction.LoginUrl = "/Identity/Account/Login";
                options.UserInteraction.LogoutUrl = "/Identity/Account/Logout";
            })
            .AddCredentialsForEnvironment(environment, gatekeeperConfig)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = dbBuilder => dbBuilder.UseMySql(dbConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = dbBuilder => dbBuilder.UseMySql(dbConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                options.EnableTokenCleanup = true;
            })
            .AddAspNetIdentity<GatekeeperUser>();

            services.AddAuthentication().AddIdentityServerAuthentication("token", options =>
            {
                options.Authority = gatekeeperConfig.GetValue<string>("OAuthAuthorityUrl");
                options.ApiName = gatekeeperConfig.GetValue<string>("ApiResourceName", "gatekeeper");
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", pb => pb.RequireClaim("user_type", "administrator"));
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
            });

            if (!environment.IsDevelopment())
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    var proxyAddresses = Dns.GetHostAddresses(gatekeeperConfig.GetValue<string>("ReverseProxyHostname", "http://nginx"));
                    foreach(var ip in proxyAddresses)
                    {
                        options.KnownProxies.Add(ip);
                    }
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ConfigurationDbContext configurationDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                var pathBase = gatekeeperConfig.GetValue<string>("PathBase", "/gatekeeper");
                RunMigrations(app);
                app.UsePathBase(pathBase);
                app.Use((context, next) =>
                {
                    context.Request.PathBase = new PathString(pathBase);
                    return next();
                });
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            GatekeeperIdentityResources.PreloadResources(configurationDbContext);

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

        private static void RunMigrations(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var serviceProvider = serviceScope.ServiceProvider;
                List<DbContext> contexts = new List<DbContext>()
                {
                    serviceProvider.GetService<GatekeeperContext>(),
                    serviceProvider.GetService<ConfigurationDbContext>(),
                    serviceProvider.GetService<PersistedGrantDbContext>(),
                };

                foreach (var context in contexts)
                {
                    context.Database.Migrate();
                    context.Dispose();
                }
            }
        }
    }
}
