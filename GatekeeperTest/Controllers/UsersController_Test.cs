using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Controllers;
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
        private readonly Mock<IUserStore<GatekeeperUser>> UserStore;
        private readonly UserManager<GatekeeperUser> UserManager;
        private readonly UsersController Controller;

        public UsersController_Test()
        {
            UserStore = new Mock<IUserStore<GatekeeperUser>>();

            // This is ugly, we only need the UserStore and UserManager only has one constructor
            UserManager = new UserManager<GatekeeperUser>(
                UserStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            Controller = new UsersController(UserManager);
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
            CancellationToken t = new CancellationToken();
            UserStore.Setup(us => us.FindByIdAsync(It.IsAny<string>(), t)).Returns<GatekeeperUser>(null);

            var result = await Controller.Get(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ReturnsOkWithValidUuid()
        {
            var expectedUser = CreateTestUser("someuuid");
            CancellationToken t = new CancellationToken();
            UserStore.Setup(us => us.FindByIdAsync("someuuid", t)).ReturnsAsync(expectedUser);

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
