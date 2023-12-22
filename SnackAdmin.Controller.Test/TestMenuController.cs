using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SnackAdmin.Controllers;
using SnackAdmin.Dtos;
using SnackAdmin.Domain;
using Moq;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dtos.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace SnackAdmin.Controller.Test
{
    public class TestMenuController
    {
        private readonly Mock<IMenuManagementLogic> _menuManagementLogicMock = new Mock<IMenuManagementLogic>();
        private readonly MenuController _controller;

        public TestMenuController()
        {
            var profile = new MenuProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            IMapper mapper = new Mapper(configuration);
            _controller = new MenuController(_menuManagementLogicMock.Object, mapper);

            var menus = new List<Menu>
            {
                new Menu(0, 1, "", "One", "", (decimal)0),
                new Menu(1, 1, "", "Two", "", (decimal)0),
                new Menu(2, 1, "", "Three", "", (decimal)0)
            };

            _menuManagementLogicMock
                .Setup(m => m.GetMenusAsync(It.IsAny<int>()))
                .ReturnsAsync((int restaurantId) =>
                {
                    if (restaurantId == 1) return menus;
                    return Enumerable.Empty<Menu>().ToList();
                });
        }

        [Theory]
        [InlineData(0, (int)HttpStatusCode.BadRequest)]
        [InlineData(1, (int)HttpStatusCode.OK)]
        [InlineData(2, (int)HttpStatusCode.BadRequest)]
        public async void GetMenusAsync_withRestaurantId_returnsObjectResult(int restaurantId, int expectedCode)
        {
            // Arrange

            // Act
            var result = await _controller.GetMenus(restaurantId);

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.NotNull(objectResult);
            Assert.Equal(expectedCode, objectResult.StatusCode);
        }
    }
}
