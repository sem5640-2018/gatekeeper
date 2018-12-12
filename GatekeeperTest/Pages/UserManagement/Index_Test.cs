using Gatekeeper.Pages.UserManagement;
using Gatekeeper.Repositories;
using GatekeeperTest.TestUtils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GatekeeperTest.Pages.UserManagement
{
    public class Index_Test
    {
        private readonly Mock<IUserRepository> userRepository;
        private readonly IndexModel page;

        public Index_Test()
        {
            userRepository = new Mock<IUserRepository>();
            page = new IndexModel(userRepository.Object);
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
            var expectedUsers = GatekeeperUserGenerator.CreateList();
            userRepository.Setup(u => u.GetAllAsync()).ReturnsAsync(expectedUsers);
            await page.OnGetAsync();
            Assert.Equal(expectedUsers, page.Users);
        }
    }
}
