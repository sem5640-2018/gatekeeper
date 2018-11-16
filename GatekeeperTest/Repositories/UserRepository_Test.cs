using Gatekeeper.Areas.Identity.Data;
using Gatekeeper.Repositories;
using GatekeeperTest.TestUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading;
using System.Security.Claims;

namespace GatekeeperTest.Repositories
{
    public class UserRepository_Test
    {
        private readonly CancellationToken t = new CancellationToken();
        private readonly Mock<IUserStore<GatekeeperUser>> userStore;

        public UserRepository_Test()
        {
            userStore = new Mock<IUserStore<GatekeeperUser>>();
        }

        private UserRepository GetRepositoryFromStore(IUserStore<GatekeeperUser> store)
        {
            var userManager = new UserManager<GatekeeperUser>(
                store, null, null, null, null, null, null, null, null
            );

            return new UserRepository(userManager);
        }

        [Fact]
        public async void DeleteAsync_RemovesUser()
        {
            var user = GatekeeperUserGenerator.Create();
            var repository = GetRepositoryFromStore(userStore.Object);
            userStore.Setup(u => u.FindByIdAsync(It.IsAny<string>(), t)).ReturnsAsync(user).Verifiable();
            userStore.Setup(u => u.DeleteAsync(user, t)).ReturnsAsync(new IdentityResult()).Verifiable();
            await repository.DeleteAsync(user.Id);
            userStore.Verify(u => u.FindByIdAsync(user.Id, t));
            userStore.Verify(u => u.DeleteAsync(user, t));
        }

        [Fact]
        public async void GetByIdAsync_ReturnsCorrectUser()
        {
            var expectedUser = GatekeeperUserGenerator.Create();
            var repository = GetRepositoryFromStore(userStore.Object);
            userStore.Setup(u => u.FindByIdAsync(expectedUser.Id, t)).ReturnsAsync(expectedUser).Verifiable();
            var user = await repository.GetByIdAsync(expectedUser.Id);
            Assert.Equal(expectedUser, user);
        }

        [Fact]
        public async void GetClaimAsync_ReturnsCorrectClaims()
        {
            var user = GatekeeperUserGenerator.Create();
            var expectedClaims = new List<Claim>()
            {
                new Claim("sometype1", "somevalue1"),
                new Claim("sometype2", "somevalue2"),
            };
            var userClaimStore = userStore.As<IUserClaimStore<GatekeeperUser>>();
            var repository = GetRepositoryFromStore(userClaimStore.Object);
            userClaimStore.Setup(u => u.GetClaimsAsync(user, t)).ReturnsAsync(expectedClaims);
            var claims = await repository.GetClaimAsync(user, "sometype1");
            Assert.Single(claims);
            Assert.Equal(expectedClaims[0], claims.First());
        }
    }
}
