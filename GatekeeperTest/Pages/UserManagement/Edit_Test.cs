using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Pages.UserManagement;
using Gatekeeper.Repositories;
using GatekeeperTest.TestUtils;
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

namespace GatekeeperTest.Pages.UserManagement
{
    public class Edit_Test
    {
        private readonly Mock<IUserRepository> userRepository;
        private readonly EditModel page;

        public Edit_Test()
        {
            userRepository = new Mock<IUserRepository>();
            page = new EditModel(userRepository.Object);
        }

        [Theory]
        [InlineData("member", "Member")]
        [InlineData("coordinator", "Coordinator")]
        [InlineData("administrator", "Administrator")]
        public void MemberTypes_ContainsCorrectTypes(string value, string text)
        {
            Assert.Contains(page.MemberTypes, i => i.Value == value && i.Text == text);
            Assert.Equal(3, page.MemberTypes.Count);
        }

        [Fact]
        public async void OnGetAsync_ReturnsNotFoundWithInvalidId()
        {
            userRepository.Setup(u => u.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((GatekeeperUser)null);
            var result = await page.OnGetAsync("any");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void OnGetAsync_ReturnsPage()
        {
            var user = GatekeeperUserGenerator.Create();
            userRepository.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            var result = await page.OnGetAsync(user.Id);
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async void OnGetAsync_SetsCorrectInputModel()
        {
            var user = GatekeeperUserGenerator.Create();
            var claims = new List<Claim>() { new Claim("user_type", "administrator") };
            userRepository.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            userRepository.Setup(u => u.GetClaimAsync(user, "user_type")).ReturnsAsync(claims);
            await page.OnGetAsync(user.Id);
            Assert.Equal(user.Email, page.Input.Email);
            Assert.Equal("administrator", page.Input.UserType);
        }

        [Fact]
        public async void OnPostAsync_ReturnsPageIfModelInvalid()
        {
            page.ModelState.AddModelError("anything", "it doesnt matter");
            var result = await page.OnPostAsync("anything");
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async void OnPostAsync_ReturnsNotFoundWithInvalidId()
        {
            userRepository.Setup(u => u.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((GatekeeperUser)null);
            var result = await page.OnPostAsync("any");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void OnPostAsync_HandlesValidInput()
        {
            var user = GatekeeperUserGenerator.Create();
            var userType = "coordinator";
            userRepository.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            userRepository.Setup(u => u.UpdateAsync(user)).Returns(Task.CompletedTask).Verifiable();
            userRepository.Setup(u => u.AddOrReplaceClaimAsync(user, It.IsAny<Claim>())).Returns(Task.CompletedTask).Verifiable();

            page.Input = new EditModel.InputModel()
            {
                Email = user.Email,
                UserType = userType
            };
            var result = await page.OnPostAsync(user.Id);
            userRepository.Verify();
            Assert.Equal("User has been updated", page.StatusMessage);
            Assert.IsType<RedirectToPageResult>(result);
        }
    }
}
