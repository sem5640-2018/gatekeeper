using Gatekeeper.Controllers;
using Gatekeeper.Repositories;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GatekeeperTest.Controllers
{
    public class ApiResoucesController_Test
    {
        private readonly Mock<IApiResourceRepository> Repository;
        private readonly ApiResourcesController Controller;

        public ApiResoucesController_Test()
        {
            Repository = new Mock<IApiResourceRepository>();
            Controller = new ApiResourcesController(Repository.Object);
        }


        [Fact]
        public async void Index_ShowCorrectView()
        {
            var result = await Controller.Index();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Index_ContainsCorrectModel()
        {
            var expectedResources = GetTestApiResources();
            Repository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedResources);

            var viewResult = await Controller.Index() as ViewResult;
            Assert.IsType<List<ApiResource>>(viewResult.Model);

            var resources = viewResult.Model as List<ApiResource>;
            Assert.Equal(expectedResources, resources);
        }

        [Fact]
        public void Create_ShowsCorrectView()
        {
            var result = Controller.Create();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Create_AddsNewApiResource()
        {
            ApiResource resource = new ApiResource()
            {
                Id = 1,
                Name = "test_resource",
                DisplayName = "Test Resource",
                Description = "It's a test resource"
            };
            Repository.Setup(r => r.AddAsync(resource)).Returns(Task.CompletedTask).Verifiable();

            var result = await Controller.Create(resource);
            Assert.IsType<RedirectToActionResult>(result);

            var redirectedResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectedResult.ActionName);

            Repository.Verify();
        }

        private List<ApiResource> GetTestApiResources()
        {
            List<ApiResource> resources = new List<ApiResource>();
            resources.Add(new ApiResource()
            {
                Id = 1,
                Name = "test_resource",
                DisplayName = "Test Resource",
                Description = "It's a test resource"
            });
            return resources;
        }
    }
}
