using Moq;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Domain;
using SnackAdmin.Controllers;
using AutoMapper;
using SnackAdmin.Dtos.Profiles;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SnackAdmin.Dtos;
using static SnackAdmin.BusinessLogic.OrderManagementLogic;
using Castle.Core.Logging;
using SnackAdmin.Services;
using ZstdSharp.Unsafe;
using Microsoft.Extensions.Configuration;

namespace SnackAdmin.Controller.Test
{
    public class TestOrderController
    {
        private readonly Mock<IOrderManagementLogic> _orderManagementLogicMock;
        private readonly Mock<WebHookController> _webHookControllerMock;
        private readonly Mock<JwtTokenService> _jwtTokenService;

        private readonly Restaurant _restaurant;
        private readonly Address _address;
        private readonly DeliveryCondition _deliveryCondition;
        private readonly List<Menu> _menus;
        private readonly List<OrderItem> _orderItems;
        private readonly Order _order;

        private readonly OrderController _controller;


        public TestOrderController()
        {
            _orderManagementLogicMock = new Mock<IOrderManagementLogic>();
            var logger = new Mock<ILogger<WebHookController>>();
            _webHookControllerMock = new Mock<WebHookController>(logger.Object);

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") // Fügen Sie Ihre Konfigurationsdateien hier hinzu
                .AddEnvironmentVariables(); // Optional: Laden von Umgebungsvariablen

            IConfiguration jwtConfiguration = configBuilder.Build();
            _jwtTokenService = new Mock<JwtTokenService>(jwtConfiguration);
            


            var orderProfile = new OrderProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(orderProfile));
            IMapper mapper = new Mapper(configuration);
            _controller = new OrderController(_orderManagementLogicMock.Object, mapper, _webHookControllerMock.Object, _jwtTokenService.Object);

            _restaurant = new Restaurant(
                1, "Burgerei", 1,
                48.370511339799776, 14.514794959548984, "",
                null, "");

            _address = new Address(
                1, "123 Main Street", "12345", "Anytown",
                "CA", "USA");

            _deliveryCondition = new DeliveryCondition(1, 1, 1, 20, 5);
                
            _menus = new List<Menu>
            {
                new Menu(1, 1, "", "one", "", (decimal)1.0),
                new Menu(2, 1, "", "two", "", (decimal)2.0),
                new Menu(3, 1, "", "three", "", (decimal)3.0),
                new Menu(4, 1, "", "four", "", (decimal)4.0),
                new Menu(5, 1, "", "five", "", (decimal)5.0),
            };
            _orderItems = new List<OrderItem>
            {
                new OrderItem(new Guid("00000000-0000-0000-0000-000000000001"), 1, 5),
                new OrderItem(new Guid("00000000-0000-0000-0000-000000000001"), 5, 2)
            };
            _order = new Order(new Guid("00000000-0000-0000-0000-000000000001"), 1, 1, 0, DateTime.Now, 48.370511339799776, 14.514794959548984, "", DeliveryStatus.OrderPlaced);

            _orderManagementLogicMock
                .Setup(logic => logic.GetMenusAsync(It.IsAny<int>()))
                .ReturnsAsync((int restaurantId) =>
                {
                    if (restaurantId == 1) return _menus;
                    else return Enumerable.Empty<Menu>().ToList();
                });

            _orderManagementLogicMock
                .Setup(logic => logic.GetOrderItemsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid orderId) =>
                {
                    if (orderId == new Guid("00000000-0000-0000-0000-000000000001")) return _orderItems;
                    else return Enumerable.Empty<OrderItem>().ToList();
                });

            _orderManagementLogicMock
                .Setup(logic => logic.GetAddressAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    if (id == 1) return _address;
                    else return null;
                });

            _orderManagementLogicMock
                .Setup(logic => logic.GetDeliveryConditionAsync(It.IsAny<Restaurant>(), It.IsAny<Order>()))
                .ReturnsAsync((Restaurant r, Order o) =>
                {
                    if (r == _restaurant && o == _order) return _deliveryCondition;
                    else return null;
                });

            _orderManagementLogicMock
                .Setup(logic => logic.GetOrderAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) =>
                {
                    if (id == new Guid("00000000-0000-0000-0000-000000000001")) return _order;
                    else return null;
                });

            _orderManagementLogicMock
                .Setup(logic => logic.GetRestaurantAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    if (id == 1) return _restaurant;
                    else return null;
                });

            _orderManagementLogicMock
                .Setup(logic => logic.AddOrderAsync(It.IsAny<Order>(), It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<Address>(), It.IsAny<String>()))
                .ReturnsAsync((Order o, IEnumerable<OrderItem> items, Address a, string token) =>
                {
                    if (o.Equals(_order) && items.Equals(_orderItems) && a.Equals(_address)) return (1, new Guid("00000000-0000-0000-0000-000000000001"));
                    else if (!items.All(item => _orderItems.Any(oi => item.MenuId == oi.MenuId))) return ((int)AddOrderFailureCode.OneOrMoreItemsNotFound, Guid.Empty);
                    else if (o.RestaurantId != _restaurant.Id) return ((int)AddOrderFailureCode.RestaurantNotFound, Guid.Empty);
                    else if (o.GpsLat == 0 || o.GpsLong == 0) return ((int)AddOrderFailureCode.MatchingDeliveryConditionNotFound, Guid.Empty);
                    else if (!items.Equals(_orderItems)) return ((int)AddOrderFailureCode.TotalCostsBelowMinOrderValue, Guid.Empty);
                    else return ((int)AddOrderFailureCode.Default, Guid.Empty);
                });

            //_webHookControllerMock
            //    .Setup(web => web.SendOrderToRestaurantWebHook(It.IsAny<string>(), It.IsAny<OrderDto>()))
            //    .ReturnsAsync(new OkResult());

            //_jwtTokenService
            //    .Setup(jwt => jwt.GenerateTokenWithStatus(It.IsAny<Order>()))
            //    .Returns("");
        }

        [Fact]
        public async void GetOrder_returnsOkObjectResult()
        {
            // Arrange

            // Act
            var result = await _controller.GetOrder(_order.Id);

            // Assert
            var okObjectResult = result.Result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
        }

        [Fact]
        public async void GetOrder_withWrongOrderId_returnsBadRequestObjectResult()
        {
            // Arrange
            var order = _order;
            order.Id = Guid.Empty;

            // Act
            var result = await _controller.GetOrder(order.Id);

            // Assert
            var badRequestObjectResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestObjectResult.StatusCode);
        }

        [Fact]
        public async void GetOrder_withWrongRestaurantId_returnsBadRequestObjectResult()
        {
            // Arrange
            var order = _order;
            order.RestaurantId = 0;

            // Act
            var result = await _controller.GetOrder(order.Id);

            // Assert
            var badRequestObjectResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestObjectResult.StatusCode);
        }
    }
}
