using Gatekeeper.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GatekeeperTest.TestUtils
{
    class GatekeeperUserGenerator
    {
        public static GatekeeperUser Create(string uuid = "a")
        {
            return new GatekeeperUser()
            {
                Id = uuid,
                UserName = $"{uuid} username",
                Email = $"{uuid}@example.com",
                PasswordHash = $"{uuid}FakeHash"
            };
        }

        public static List<GatekeeperUser> CreateList(int length = 5)
        {
            List<GatekeeperUser> users = new List<GatekeeperUser>();
            for (var i = 0; i < length; i++)
            {
                users.Add(Create(i.ToString()));
            }
            return users;
        }
    }
}
