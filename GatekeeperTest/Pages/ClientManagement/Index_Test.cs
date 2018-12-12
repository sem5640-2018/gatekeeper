using Gatekeeper.Pages.ClientManagement;
using Gatekeeper.Repositories;
using GatekeeperTest.TestUtils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GatekeeperTest.Pages.ClientManagement
{
    public class Index_Test
    {
        private readonly Mock<IClientRepository> clientRepository;
        private readonly IndexModel page;

        public Index_Test()
        {
            clientRepository = new Mock<IClientRepository>();
            page = new IndexModel(clientRepository.Object);
        }

        [Fact]
        public async void OnGetAsync_ResolvesToPageResult()
        {
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async void OnGetAsync_GetsAllUsers()
        {
            var expectedClients = ClientGenerator.CreateList();
            clientRepository.Setup(u => u.GetAllAsync()).ReturnsAsync(expectedClients);
            await page.OnGetAsync();
            Assert.Equal(expectedClients, page.Clients);
        }
    }
}
