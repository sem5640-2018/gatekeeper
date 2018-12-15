using AberFitnessAuditLogger;
using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Pages.UserManagement;
using Gatekeeper.Repositories;
using GatekeeperTest.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GatekeeperTest.Pages.UserManagement
{
    public class Delete_Test
    {
        private readonly Mock<IUserRepository> userRepository;
        private readonly Mock<IAuditLogger> auditLogger;
        private readonly DeleteModel page;

        public Delete_Test()
        {
            userRepository = new Mock<IUserRepository>();
            auditLogger = new Mock<IAuditLogger>();
            page = new DeleteModel(userRepository.Object, auditLogger.Object);
        }

        [Fact]
        public async void OnGetAsync_ReturnsNotFoundWithInvalidId()
        {
            userRepository.Setup(u => u.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((GatekeeperUser)null);
            var result = await page.OnGetAsync("any id");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void OnGetAsync_ReturnsPageResult()
        {
            var user = GatekeeperUserGenerator.Create();
            userRepository.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            var result = await page.OnGetAsync(user.Id);
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async void OnGetAsync_ShowsCorrectUser()
        {
            var user = GatekeeperUserGenerator.Create();
            userRepository.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            var result = await page.OnGetAsync(user.Id);
            Assert.Equal(user, page.UserToDelete);
        }

        [Fact]
        public async void OnPostAsync_ReturnsNotFoundWithInvalidId()
        {
            userRepository.Setup(u => u.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((GatekeeperUser)null);
            var result = await page.OnPostAsync("any id");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void OnPostAsync_DeletesUser()
        {
            var user = GatekeeperUserGenerator.Create();
            userRepository.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            userRepository.Setup(u => u.DeleteAsync(user.Id)).Returns(Task.CompletedTask).Verifiable();
            var result = await page.OnPostAsync(user.Id);
            userRepository.Verify();
        }

        [Fact]
        public async void OnPostAsync_RedirectsToIndex()
        {
            var user = GatekeeperUserGenerator.Create();
            userRepository.Setup(u => u.GetByIdAsync(user.Id)).ReturnsAsync(user);
            userRepository.Setup(u => u.DeleteAsync(user.Id)).Returns(Task.CompletedTask).Verifiable();
            var result = await page.OnPostAsync(user.Id);
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("Index", redirectResult.PageName);
        }
    }
}
