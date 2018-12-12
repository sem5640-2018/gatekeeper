using Gatekeeper.Repositories;
using GatekeeperTest.TestUtils;
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
        private static readonly Random random = new Random();
        private DbContextOptions<ConfigurationDbContext> ContextOptions;
        private ConfigurationStoreOptions StoreOptions;

        public ApiResourceRepository_Test()
        {
            ContextOptions = new DbContextOptionsBuilder<ConfigurationDbContext>()
                .UseInMemoryDatabase($"rand_db_name_{random.Next()}")
                .Options;

            StoreOptions = new ConfigurationStoreOptions();
        }

        [Fact]
        public async void AddAsync_AddsToContext()
        {
            var resource = ApiResourceGenerator.Create();
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
            var resource = ApiResourceGenerator.Create();
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

        [Fact]
        public async void GetAllSync_ReturnsAllFromContext()
        {
            var expectedResources = ApiResourceGenerator.CreateList();
            using (var context = new ConfigurationDbContext(ContextOptions, StoreOptions))
            {
                context.Database.EnsureCreated();
                context.ApiResources.AddRange(expectedResources);
                context.SaveChanges();
                Assert.Equal(expectedResources.Count, await context.ApiResources.CountAsync());
                var repository = new ApiResourceRepository(context);
                var resources = await repository.GetAllAsync();
                Assert.IsType<List<ApiResource>>(resources);
                Assert.Equal(expectedResources, resources);
            }
        }

        [Fact]
        public async void GetByIdAsync_ReturnsCorrectItems()
        {
            var resources = ApiResourceGenerator.CreateList(5);
            var expectedResource = resources[2];
            using (var context = new ConfigurationDbContext(ContextOptions, StoreOptions))
            {
                context.Database.EnsureCreated();
                context.ApiResources.AddRange(resources);
                context.SaveChanges();
                Assert.Equal(resources.Count, await context.ApiResources.CountAsync());
                var repository = new ApiResourceRepository(context);
                var resource = await repository.GetByIdAsync(expectedResource.Id);
                Assert.IsType<ApiResource>(resource);
                Assert.Equal(expectedResource, resource);
            }
        }
    }
}
