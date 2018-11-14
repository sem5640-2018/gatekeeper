using Gatekeeper.Repositories;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GatekeeperTest.Repositories
{
    public class ApiResourceRepository_Test
    {
        private DbContextOptions<ConfigurationDbContext> ContextOptions;
        private ConfigurationStoreOptions StoreOptions;

        public ApiResourceRepository_Test()
        {
            ContextOptions = new DbContextOptionsBuilder<ConfigurationDbContext>()
                .UseInMemoryDatabase("database_name")
                .Options;

            StoreOptions = new ConfigurationStoreOptions();
        }

        [Fact]
        public async void AddAsync_AddsToContext()
        {
            var resource = new ApiResource() { Name = "test_resource" };
            using (var context = new ConfigurationDbContext(ContextOptions, StoreOptions))
            {
                context.Database.EnsureCreated();
                var repository = new ApiResourceRepository(context);
                await repository.AddAsync(resource);
                Assert.Equal(1, await context.ApiResources.CountAsync());
                Assert.Equal(resource, await context.ApiResources.SingleAsync());
            }
        }

        [Fact]
        public async void DeleteAsync_RemovesFromContext()
        {
            var resource = new ApiResource() { Name = "test_resource" };
            using (var context = new ConfigurationDbContext(ContextOptions, StoreOptions))
            {
                context.Database.EnsureCreated();
                context.ApiResources.Add(resource);
                context.SaveChanges();
                Assert.Equal(1, await context.ApiResources.CountAsync());
                var repository = new ApiResourceRepository(context);
                await repository.DeleteAsync(resource.Id);
                Assert.Equal(0, await context.ApiResources.CountAsync());
            }
        }
    }
}
