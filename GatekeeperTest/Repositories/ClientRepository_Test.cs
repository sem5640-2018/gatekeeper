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
    public class ClientRepository_Test
    {
        private static readonly Random random = new Random();
        private DbContextOptions<ConfigurationDbContext> contextOptions;
        private ConfigurationStoreOptions storeOptions;

        public ClientRepository_Test()
        {
            contextOptions = new DbContextOptionsBuilder<ConfigurationDbContext>()
                .UseInMemoryDatabase($"rand_db_name_{random.Next()}")
                .Options;

            storeOptions = new ConfigurationStoreOptions();
        }

        [Fact]
        public async void AddAsync_AddsToContext()
        {
            var client = ClientGenerator.Create();
            using (var context = new ConfigurationDbContext(contextOptions, storeOptions))
            {
                context.Database.EnsureCreated();
                var repository = new ClientRepository(context);
                await repository.AddAsync(client);
                Assert.Equal(1, await context.Clients.CountAsync());
                Assert.Equal(client, await context.Clients.SingleAsync());
            }
        }

        [Fact]
        public async void DeleteAsync_RemovesFromContext()
        {
            var client = ClientGenerator.Create();
            using (var context = new ConfigurationDbContext(contextOptions, storeOptions))
            {
                context.Database.EnsureCreated();
                context.Clients.Add(client);
                context.SaveChanges();
                Assert.Equal(1, await context.Clients.CountAsync());
                var repository = new ClientRepository(context);
                await repository.DeleteAsync(client.Id);
                Assert.Equal(0, await context.Clients.CountAsync());
            }
        }

        [Fact]
        public async void GetAllSync_ReturnsAllFromContext()
        {
            var expectedClient = ClientGenerator.CreateList();
            using (var context = new ConfigurationDbContext(contextOptions, storeOptions))
            {
                context.Database.EnsureCreated();
                context.Clients.AddRange(expectedClient);
                context.SaveChanges();
                Assert.Equal(expectedClient.Count, await context.Clients.CountAsync());
                var repository = new ClientRepository(context);
                var clients = await repository.GetAllAsync();
                Assert.IsType<List<Client>>(clients);
                Assert.Equal(expectedClient, clients);
            }
        }

        [Fact]
        public async void GetByIdAsync_ReturnsCorrectItems()
        {
            var clients = ClientGenerator.CreateList(5);
            var expectedClient = clients[2];
            using (var context = new ConfigurationDbContext(contextOptions, storeOptions))
            {
                context.Database.EnsureCreated();
                context.Clients.AddRange(clients);
                context.SaveChanges();
                Assert.Equal(clients.Count, await context.Clients.CountAsync());
                var repository = new ClientRepository(context);
                var client = await repository.GetByIdAsync(expectedClient.Id);
                Assert.IsType<Client>(client);
                Assert.Equal(expectedClient, client);
            }
        }
    }
}
