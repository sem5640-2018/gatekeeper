using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GatekeeperTest.TestUtils
{
    public class ApiResourceGenerator
    {
        public static ApiResource Create(int index = 0)
        {
            return new ApiResource()
            {
                Name = $"test_resource_{index}",
                DisplayName = $"Test Resource {index}",
                Description = $"It's a test resource {index}"
            };
        }

        public static List<ApiResource> CreateList(int length = 5)
        {
            List<ApiResource> resources = new List<ApiResource>();
            for (var i = 0; i < length; i++)
            {
                resources.Add(Create(i));
            }
            return resources;
        }
    }
}
