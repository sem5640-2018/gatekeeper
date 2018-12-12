using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Pages.ClientManagement;
using Gatekeeper.Repositories;
using GatekeeperTest.TestUtils;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GatekeeperTest.Pages.ClientManagement
{
    public class Delete_Test
    {
        private readonly Mock<IClientRepository> clientRepository;
        private readonly DeleteModel page;

        public Delete_Test()
        {
            clientRepository = new Mock<IClientRepository>();
            page = new DeleteModel(clientRepository.Object);
        }

        [Fact]
        public async void OnGetAsync_ReturnsNotFoundWithInvalidId()
        {
            clientRepository.Setup(u => u.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Client)null);
            var result = await page.OnGetAsync(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void OnGetAsync_ReturnsPageResult()
        {
            var client = ClientGenerator.Create();
            clientRepository.Setup(u => u.GetByIdAsync(client.Id)).ReturnsAsync(client);
            var result = await page.OnGetAsync(client.Id);
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async void OnGetAsync_ShowsCorrectClient()
        {
            var client = ClientGenerator.Create();
            clientRepository.Setup(u => u.GetByIdAsync(client.Id)).ReturnsAsync(client);
            var result = await page.OnGetAsync(client.Id);
            Assert.Equal(client, page.ClientToDelete);
        }

        [Fact]
        public async void OnPostAsync_ReturnsNotFoundWithInvalidId()
        {
            clientRepository.Setup(u => u.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Client)null);
            var result = await page.OnPostAsync(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void OnPostAsync_DeletesClient()
        {
            var client = ClientGenerator.Create();
            clientRepository.Setup(u => u.GetByIdAsync(client.Id)).ReturnsAsync(client);
            clientRepository.Setup(u => u.DeleteAsync(client.Id)).Returns(Task.CompletedTask).Verifiable();
            var result = await page.OnPostAsync(client.Id);
            clientRepository.Verify();
        }

        [Fact]
        public async void OnPostAsync_RedirectsToIndex()
        {
            var client = ClientGenerator.Create();
            clientRepository.Setup(u => u.GetByIdAsync(client.Id)).ReturnsAsync(client);
            clientRepository.Setup(u => u.DeleteAsync(client.Id)).Returns(Task.CompletedTask).Verifiable();
            var result = await page.OnPostAsync(client.Id);
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("Index", redirectResult.PageName);
        }
    }
}
