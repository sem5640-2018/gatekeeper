using Gatekeeper.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GatekeeperTest.Repositories
{
    class UserRepository_Test
    {
        private readonly Mock<IUserStore<GatekeeperUser>> UserStore;
        private readonly UserManager<GatekeeperUser> UserManager;

        public UserRepository_Test()
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
        }
    }
}
