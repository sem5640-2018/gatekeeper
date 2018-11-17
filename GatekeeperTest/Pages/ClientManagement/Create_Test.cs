using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Pages.ClientManagement;
using Gatekeeper.Repositories;
using GatekeeperTest.TestUtils;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GatekeeperTest.Pages.ClientManagement
{
    public class Create_Test
    {
        private readonly Mock<IClientRepository> clientRepository;
        private readonly CreateModel page;

        public Create_Test()
        {
            clientRepository = new Mock<IClientRepository>();
            page = new CreateModel(clientRepository.Object);
        }

        [Theory]
        [InlineData("hybrid", "Hybrid")]
        [InlineData("client_credentials", "Client Credentials")]
        [InlineData("authorization_code", "Code")]
        [InlineData("implicit", "Implicit")]
        public void AvailableGrantTypes_ContainsCorrectGrants(string value, string text)
        {
            Assert.Contains(page.AvailableGrantTypes, i => i.Value == value && i.Text == text);
            Assert.Equal(4, page.AvailableGrantTypes.Count);
        }

        [Fact]
        public void OnGet_ReturnsPage()
        {
            var result = page.OnGet();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void OnGet_CreatesInputModel()
        {
            page.OnGet();
            Assert.IsType<CreateModel.InputModel>(page.Input);
            Assert.NotNull(page.Input);
        }

        [Fact]
        public async void OnPostAsync_ReturnsPageIfModelInvalid()
        {
            page.ModelState.AddModelError("anything", "it doesnt matter");
            var result = await page.OnPostAsync();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async void OnPostAsync_HandlesValidInput()
        {
            clientRepository.Setup(u => u.AddAsync(It.IsAny<Client>())).Returns(Task.CompletedTask).Verifiable();

            page.Input = new CreateModel.InputModel()
            {
                ClientId = "test",
                ClientName = "Test Client",
                GrantTypes = new List<string>{ "hybrid", "client_credentials" },
                RedirectUri = "https://example.com",
                Scopes = "openid profile",
                ClientSecret = "testsecret"

            };
            var result = await page.OnPostAsync();
            clientRepository.Verify();
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("Index", redirectResult.PageName);
        }
    }
}
