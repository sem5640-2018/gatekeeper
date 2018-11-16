using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Controllers;
using Gatekeeper.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace GatekeeperTest.Controllers
{
    public class UsersController_Test
    {
        private readonly Mock<IUserRepository> Repository;
        private readonly UsersController Controller;

        public UsersController_Test()
        {
            Repository = new Mock<IUserRepository>();
            Controller = new UsersController(Repository.Object);
        }

        [Fact]
        public async void Get_ReturnsNotFoundWithNullUuid()
        {
            var result = await Controller.Get(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ReturnsNotFoundWithInvalidUuid()
        {
            Repository.Setup(r => r.GetByIdAsync(It.IsAny<string>())).Returns<GatekeeperUser>(null);

            var result = await Controller.Get(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ReturnsOkWithValidUuid()
        {
            var expectedUser = CreateTestUser("someuuid");
            Repository.Setup(r => r.GetByIdAsync("someuuid")).ReturnsAsync(expectedUser);

            var result = await Controller.Get("someuuid");
            Assert.IsType<OkObjectResult>(result);
            var content = result as OkObjectResult;
            Assert.IsType<Dictionary<string, string>>(content.Value);
            var dict = content.Value as Dictionary<string, string>;

            var expectedUserDict = new Dictionary<string, string>() {
                { "id", expectedUser.Id },
                { "email", expectedUser.Email },
                { "name", expectedUser.UserName },
            };

            Assert.Equal(expectedUserDict, dict);
        }

        private GatekeeperUser CreateTestUser(string uuid)
        {
            return new GatekeeperUser()
            {
                Id = uuid,
                UserName = "A username",
                Email = "user@example.com",
                PasswordHash = "a fake hash"
            };
        }
    }
}
