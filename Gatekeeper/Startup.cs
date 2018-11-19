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
                options.UseMySql(dbConnectionString));

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

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(Dns.GetHostEntry("http://nginx").AddressList[0]);
            });

            services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ConfigurationDbContext configurationDbContext)
        {
            UpdateDatabase(app);
            GatekeeperIdentityResources.PreloadResources(configurationDbContext);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UsePathBase("/gatekeeper");
                app.Use((context, next) =>
                {
                    context.Request.PathBase = new PathString("/gatekeeper");
                    return next();
                });
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
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

        private static void UpdateDatabase(IApplicationBuilder app)
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
