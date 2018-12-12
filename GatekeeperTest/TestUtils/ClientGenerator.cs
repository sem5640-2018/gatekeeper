using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GatekeeperTest.TestUtils
{
    public class ClientGenerator
    {
        public static Client Create(int index = 0)
        {
            return new Client()
            {
                ClientId = $"test_client_{index}",
                ClientName = $"Test Client {index}",
                Description = $"It's a test client {index}"
            };
        }

        public static List<Client> CreateList(int length = 5)
        {
            List<Client> resources = new List<Client>();
            for (var i = 0; i < length; i++)
            {
                resources.Add(Create(i));
            }
            return resources;
        }
    }
}
