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
            var expectedResources = CreateTestApiResourcesList();
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
            var resource = CreateTestApiResource();
            Repository.Setup(r => r.AddAsync(resource)).Returns(Task.CompletedTask).Verifiable();

            var result = await Controller.Create(resource);
            Assert.IsType<RedirectToActionResult>(result);

            var redirectedResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectedResult.ActionName);

            Repository.Verify();
        }

        [Fact]
        public async void Delete_ShowsCorrectView()
        {
            Repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CreateTestApiResource());
            var result = await Controller.Delete(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Delete_ContainsCorrectModel()
        {
            var expectedResource = CreateTestApiResource();
            Repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Delete(1) as ViewResult;
            Assert.IsType<ApiResource>(viewResult.Model);

            var resources = viewResult.Model as ApiResource;
            Assert.Equal(expectedResource, resources);
        }

        [Fact]
        public async void DeleteConfirmed_DeletesApiResource()
        {
            var resource = CreateTestApiResource();
            Repository.Setup(r => r.DeleteAsync(resource.Id)).Returns(Task.CompletedTask).Verifiable();

            var result = await Controller.DeleteConfirmed(resource.Id);
            Assert.IsType<RedirectToActionResult>(result);

            var redirectedResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectedResult.ActionName);

            Repository.Verify();
        }

        private List<ApiResource> CreateTestApiResourcesList(int length = 5)
        {
            List<ApiResource> resources = new List<ApiResource>();
            for (var i = 0; i < length; i++)
            {
                resources.Add(CreateTestApiResource(i));
            }
            return resources;
        }

        private ApiResource CreateTestApiResource(int id = 0)
        {
            return new ApiResource()
            {
                Id = id,
                Name = $"test_resource_{id}",
                DisplayName = "Test Resource {id}",
                Description = "It's a test resource {id}"
            };
        }
    }
}
