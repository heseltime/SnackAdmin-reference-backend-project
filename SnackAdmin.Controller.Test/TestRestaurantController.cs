using System.Net;
using SnackAdmin.Controllers;
using SnackAdmin.Dtos;
using SnackAdmin.Domain;
using Moq;
using SnackAdmin.BusinessLogic.Interfaces;
using AutoMapper;
using SnackAdmin.Dtos.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace SnackAdmin.Controller.Test
{
    public class TestRestaurantController
    {
        private readonly Mock<IRestaurantManagementLogic> _restaurantManagementLogicMock;

        public TestRestaurantController()
        {
            var restaurants = new List<Restaurant>
            {
                new Restaurant(
                    1, "Latino Bar", 1,
                    48.52231449370366, 14.294476125935876, "http://test.url",
                    null, ""),
                new Restaurant(
                    2, "Linz", 1,
                    48.30639, 14.28611, "http://test.url",
                    null, ""),
                new Restaurant(
                    3, "Null Island", 1,
                    0, 0, "http://test.url",
                    null, "")
            };

            var address = new Address(
                100, "808 Test Street", "4813", "Altmuenster",
                "Upper Austria", "Austria");

            _restaurantManagementLogicMock = new Mock<IRestaurantManagementLogic>();
            _restaurantManagementLogicMock.Setup(rl => rl.GetRestaurantsAsync(0, 0, 0)).ReturnsAsync(restaurants);
            _restaurantManagementLogicMock.Setup(rl => rl.GetAddressByIdAsync(1)).ReturnsAsync(address);
        }


        [Fact]
        public async void GetRestaurants_returnsRestaurantDtos()
        {
            // Arrange
            var restaurantProfile = new RestaurantProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(restaurantProfile));
            IMapper mapper = new Mapper(configuration);
            var controller = new RestaurantController(_restaurantManagementLogicMock.Object, mapper);

            // Act
            var actionResult = await controller.GetRestaurants(0, 0, 0);
            var result = actionResult.Result as OkObjectResult;
            var restaurantDtos = result.Value as IEnumerable<RestaurantDto>;

            // Assert
            Assert.NotNull(result); // Checks if the result is not null
            Assert.IsType<OkObjectResult>(result); // Checks if the result is of type OkObjectResult
            Assert.NotNull(restaurantDtos); // Checks if the restaurantDtos is not null
            Assert.Equal(3, restaurantDtos.Count()); // Checks the count of returned RestaurantDtos
            Assert.All(restaurantDtos, dto => Assert.IsType<RestaurantDto>(dto)); // Checks if all items are of type RestaurantDto
        }


        [Theory]
        [InlineData(-200, 0, 0)]
        [InlineData(200, 0, 0)]
        [InlineData(0, -200, 0)]
        [InlineData(0, 200, 0)]
        [InlineData(0, 0, -1)]
        [InlineData(0, 0, 15000)]
        public async Task GetRestaurants_ReturnsBadRequestForInvalidInput(double longitude, double latitude, int radius)
        {
            // Arrange
            var restaurantManagementLogicMock = new Mock<IRestaurantManagementLogic>();
            var mapperMock = new Mock<IMapper>();

            var controller = new RestaurantController(restaurantManagementLogicMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetRestaurants(longitude, latitude, radius);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result); // Asserts that the result is of type ObjectResult
            var badRequestObjectResult = result.Result as BadRequestObjectResult;
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestObjectResult.StatusCode); // Asserts the status code is BadRequest
        }
    }
}