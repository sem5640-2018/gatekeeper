using Gatekeeper.Util;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GatekeeperTest.Util
{
    public class GatekeeperIdentityResources_Test
    {
        private static readonly Random random = new Random();
        private DbContextOptions<ConfigurationDbContext> ContextOptions;
        private ConfigurationStoreOptions StoreOptions;

        public GatekeeperIdentityResources_Test()
        {
            ContextOptions = new DbContextOptionsBuilder<ConfigurationDbContext>()
                .UseInMemoryDatabase($"rand_db_name_{random.Next()}")
                .Options;

            StoreOptions = new ConfigurationStoreOptions();
        }

        [Fact]
        public async void PreloadResources_DoesNothingWhenResourcesExist()
        {
            using (var context = new ConfigurationDbContext(ContextOptions, StoreOptions))
            {
                var existingResource = new IdentityResource() { Name = "Test" };
                context.Database.EnsureCreated();
                context.IdentityResources.Add(existingResource);
                context.SaveChanges();
                Assert.Equal(1, await context.IdentityResources.CountAsync());
                GatekeeperIdentityResources.PreloadResources(context);
                Assert.Equal(1, await context.IdentityResources.CountAsync());
                Assert.Equal(existingResource, await context.IdentityResources.SingleAsync());
            }
        }

        [Fact]
        public async void PreloadResources_SeedsResources()
        {
            using (var context = new ConfigurationDbContext(ContextOptions, StoreOptions))
            {
                var expectedResources = GatekeeperIdentityResources.GetIdentityResources();
                var expectedResourcesList = expectedResources as List<IdentityResource>;
                context.Database.EnsureCreated();
                Assert.Equal(0, await context.IdentityResources.CountAsync());
                GatekeeperIdentityResources.PreloadResources(context);
                Assert.Equal(expectedResourcesList.Count, await context.IdentityResources.CountAsync());
            }
        }
    }
}
