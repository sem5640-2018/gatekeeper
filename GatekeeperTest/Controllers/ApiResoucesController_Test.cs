using Gatekeeper.Controllers;
using Gatekeeper.Repositories;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace GatekeeperTest.Controllers
{
    public class ApiResoucesController_Test
    {
        [Fact]
        public async void Index_ContainsCorrectModel()
        {
            var repository = new Mock<IApiResourceRepository>();
            var expectedResources = GetTestApiResources();
            repository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedResources);

            var controller = new ApiResourcesController(repository.Object);
            var result = await controller.Index();
            Assert.IsType<ViewResult>(result);

            var viewResult = result as ViewResult;
            Assert.IsType<List<ApiResource>>(viewResult.Model);

            var resources = viewResult.Model as List<ApiResource>;
            Assert.Equal(expectedResources, resources);
        }

        private List<ApiResource> GetTestApiResources()
        {
            List<ApiResource> resources = new List<ApiResource>();
            resources.Add(new ApiResource()
            {
                Name = "test_resource",
                DisplayName = "Test Resource",
                Description = "It's a test resource"
            });
            return resources;
        }
    }
}
