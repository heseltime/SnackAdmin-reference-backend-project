using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Moq;
using SnackAdmin.Domain;
using SnackAdmin.Dal.Interface;
using SnackAdmin.BusinessLogic.Interfaces;
using static SnackAdmin.BusinessLogic.OrderManagementLogic;

namespace SnackAdmin.BusinessLogic.Test
{
    public class TestOrderManagementLogic
    {
        private readonly Mock<IRestaurantDao> _restaurantDaoMock;
        private readonly Mock<IAddressDao> _addressDaoMock;
        private readonly Mock<IDeliveryCondition> _deliveryConditionDaoMock;
        private readonly Mock<IMenuDao> _menuDaoMock;
        private readonly Mock<IOrderItemDao> _orderItemDaoMock;
        private readonly Mock<IOrderDao> _orderDaoMock;
        private readonly Mock<IOpeningHourDao> _hourDaoMock;

        private readonly Restaurant _restaurant;
        private readonly Address _address;
        private readonly List<DeliveryCondition> _deliveryConditions;
        private readonly List<Menu> _menus;
        private readonly List<OrderItem> _orderItems;
        private readonly List<OpeningHour> _hours;
        private readonly Order _order;

        private readonly OrderManagementLogic _orderManagementLogic;

        public TestOrderManagementLogic()
        {
            _restaurantDaoMock = new Mock<IRestaurantDao>();
            _addressDaoMock = new Mock<IAddressDao>();
            _deliveryConditionDaoMock = new Mock<IDeliveryCondition>();
            _menuDaoMock = new Mock<IMenuDao>();
            _orderItemDaoMock = new Mock<IOrderItemDao>();
            _orderDaoMock = new Mock<IOrderDao>();
            _hourDaoMock = new Mock<IOpeningHourDao>();

            _orderManagementLogic = new OrderManagementLogic(
                _orderItemDaoMock.Object, _orderDaoMock.Object, _restaurantDaoMock.Object,
                _addressDaoMock.Object, _menuDaoMock.Object, _deliveryConditionDaoMock.Object, _hourDaoMock.Object);

            _restaurant = new Restaurant(
                1, "Burgerei", 1,
                48.370511339799776, 14.514794959548984, "",
                null, "");

            _address = new Address(
                1, "123 Main Street", "12345", "Anytown",
                "CA", "USA");

            _deliveryConditions = new List<DeliveryCondition>
            {
                new DeliveryCondition(1, 1, 1, 20, 5),
                new DeliveryCondition(2, 1, 50, 30, 5),
                new DeliveryCondition(3, 1, 1000, 40, 5)
            };
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

            _hours = new List<OpeningHour>
            {
                new OpeningHour(1,1,Day.Monday, new TimeSpan(7, 0, 0), new TimeSpan(23, 0, 0)),
                new OpeningHour(1,1,Day.Tuesday, new TimeSpan(7, 0, 0), new TimeSpan(23, 0, 0)),
                new OpeningHour(1,1,Day.Wednesday, new TimeSpan(7, 0, 0), new TimeSpan(23, 0, 0)),
                new OpeningHour(1,1,Day.Thursday, new TimeSpan(7, 0, 0), new TimeSpan(23, 0, 0)),
                new OpeningHour(1,1,Day.Friday, new TimeSpan(7, 0, 0), new TimeSpan(23, 0, 0)),
                new OpeningHour(1,1,Day.Saturday, new TimeSpan(7, 0, 0), new TimeSpan(23, 0, 0)),
                new OpeningHour(1,1,Day.Sonday, new TimeSpan(7, 0, 0), new TimeSpan(23, 0, 0))
            };

            _restaurantDaoMock.
                Setup(dao => dao.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    if (id == 1) return _restaurant;
                    else return null;
                });

            _addressDaoMock.
                Setup(dao => dao.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    if (id == 1) return _address;
                    else return null;
                });
            _addressDaoMock.
                Setup(dao => dao.InsertAsync(It.IsAny<Address>()))
                .ReturnsAsync((Address a) =>
                {
                    if (a == _address) return 1;
                    else return 0;
                });
            _addressDaoMock.
                Setup(dao => dao.DeleteAsync(It.IsAny<Address>()))
                .ReturnsAsync((Address a) =>
                {
                    if (a == _address) return true;
                    else return false;
                });

            _deliveryConditionDaoMock.
                Setup(dao => dao.FindAllByRestaurantIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    if (id == 1) return _deliveryConditions;
                    else return Enumerable.Empty<DeliveryCondition>().ToList();
                });

            _menuDaoMock.
                Setup(dao => dao.FindAllByRestaurantIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    if (id == 1) return _menus;
                    else return Enumerable.Empty<Menu>().ToList();
                });

            _orderItemDaoMock.
                Setup(dao => dao.FindAllByOrderIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) =>
                {
                    if (id == new Guid("00000000-0000-0000-0000-000000000001")) return _orderItems;
                    else return Enumerable.Empty<OrderItem>().ToList();
                });
            _orderItemDaoMock.
                Setup(dao => dao.InsertAsync(It.IsAny<OrderItem>()))
                .ReturnsAsync(1);

            _orderDaoMock.
                Setup(dao => dao.FindByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) =>
                {
                    if (id == new Guid("00000000-0000-0000-0000-000000000001")) return _order;
                    else return null;
                }); 
            _orderDaoMock.
                Setup(dao => dao.InsertForGuidAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order o) =>
                {
                    if (o == _order) return new Guid("00000000-0000-0000-0000-000000000001");
                    else return Guid.Empty;
                });

            _hourDaoMock
                .Setup(h => h.FindAllByRestaurantIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_hours);
        }

        //**********************************************************************
        [Fact]
        public async void GetOrderAsync_withValidId_returnsOrder()
        {
            // Arrange

            // Act
            var order = await _orderManagementLogic.GetOrderAsync(new Guid("00000000-0000-0000-0000-000000000001"));

            // Assert
            Assert.Equal(order, _order);
        }

        [Fact]
        public async void GetOrderAsync_withInalidId_returnsNull()
        {
            // Arrange

            // Act
            var order = await _orderManagementLogic.GetOrderAsync(Guid.Empty);

            // Assert
            Assert.Null(order);
        }

        //**********************************************************************
        [Fact]
        public async void GetOrderItemsAsync_withValidId_returnsOrder()
        {
            // Arrange

            // Act
            var items = await _orderManagementLogic.GetOrderItemsAsync(new Guid("00000000-0000-0000-0000-000000000001"));

            // Assert
            Assert.Equal(items, _orderItems);
        }

        [Fact]
        public async void GetOrderItemsAsync_withInalidId_returnsNull()
        {
            // Arrange

            // Act
            var items = await _orderManagementLogic.GetOrderItemsAsync(Guid.Empty);

            // Assert
            Assert.False(items.Any());
        }

        //**********************************************************************
        [Fact]
        public async void GetMenusAsync_withValidId_returnsOrder()
        {
            // Arrange

            // Act
            var items = await _orderManagementLogic.GetMenusAsync(1);

            // Assert
            Assert.Equal(items, _menus);
        }

        [Fact]
        public async void GetMenusAsync_withInalidId_returnsNull()
        {
            // Arrange

            // Act
            var items = await _orderManagementLogic.GetMenusAsync(0);

            // Assert
            Assert.False(items.Any());
        }

        //**********************************************************************
        [Fact]
        public async void GetAddressAsync_withValidId_returnsOrder()
        {
            // Arrange

            // Act
            var value = await _orderManagementLogic.GetAddressAsync(1);

            // Assert
            Assert.Equal(value, _address);
        }

        [Fact]
        public async void GetAddressAsync_withInalidId_returnsNull()
        {
            // Arrange

            // Act
            var value = await _orderManagementLogic.GetAddressAsync(0);

            // Assert
            Assert.Null(value);
        }

        //**********************************************************************
        [Fact]
        public async void GetRestaurantAsync_withValidId_returnsOrder()
        {
            // Arrange

            // Act
            var value = await _orderManagementLogic.GetRestaurantAsync(1);

            // Assert
            Assert.Equal(value, _restaurant);
        }

        [Fact]
        public async void GetRestaurantAsync_withInalidId_returnsNull()
        {
            // Arrange

            // Act
            var value = await _orderManagementLogic.GetRestaurantAsync(0);

            // Assert
            Assert.Null(value);
        }

        //**********************************************************************
        [Theory]
        [InlineData(48.370511339799776, 14.514794959548984, 1)]  // Hagenberg
        [InlineData(48.52304613664876, 14.291985650078145, 2)] // Bad Leonfelden
        [InlineData(47.810047162234994, 13.05591192929201, 3)]  // Salzburg
        public async void GetDeliveryConditionAsync_withinRange_returnsCondition(double latitude, double longitude, int expectedConditionId)
        {
            // Arrange
            var restaurant = _restaurant;
            var order = _order;
            order.GpsLat = latitude;
            order.GpsLong = longitude;

            // Act
            var condition = await _orderManagementLogic.GetDeliveryConditionAsync(restaurant, order);

            // Assert
            Assert.NotNull(condition);
            Assert.Equal(expectedConditionId, condition.Id);
        }

        [Fact]
        public async void GetDeliveryConditionAsync_outOfRange_returnsNull()
        {
            // Arrange
            var restaurant = _restaurant;
            var order = _order;
            order.GpsLat = 40.71246633972071;
            order.GpsLong = -74.00234892875378;

            // Act
            var condition = await _orderManagementLogic.GetDeliveryConditionAsync(restaurant, order);

            // Assert
            Assert.Null(condition);
        }

        //**********************************************************************
        [Fact]
        public async Task AddOrderAsync_SuccessfulOrder()
        {
            // Arrange
            var order = _order;
            var items = _orderItems;
            items.Add(new OrderItem(new Guid("00000000-0000-0000-0000-000000000001"), 5,10));
            var address = _address;
            var token = "";

            // Act
            var result = await _orderManagementLogic.AddOrderAsync(order, items, address, token);

            // Assert
            Assert.Equal(1, result.Item1);
        }

        [Fact]
        public async Task AddOrderAsync_RestaurantNotFound()
        {
            // Arrange
            var order = _order;
            order.RestaurantId = 0;
            var items = _orderItems;
            var address = _address;
            var token = "";

            // Act
            var result = await _orderManagementLogic.AddOrderAsync(order, items, address, token);

            // Assert
            Assert.Equal((int)AddOrderFailureCode.RestaurantNotFound, result.Item1);
        }

        [Fact]
        public async Task AddOrderAsync_OneOrMoreItemsNotFound()
        {
            // Arrange
            var order = _order;
            var items = _orderItems;
            items[0].MenuId = 0;
            var address = _address;
            var token = "";

            // Act
            var result = await _orderManagementLogic.AddOrderAsync(order, items, address, token);

            // Assert
            Assert.Equal((int)AddOrderFailureCode.OneOrMoreItemsNotFound, result.Item1);
        }

        [Fact]
        public async Task AddOrderAsync_MatchingDeliveryConditionNotFound()
        {
            // Arrange
            var order = _order;
            order.GpsLat = 0;
            var items = _orderItems;
            var address = _address;
            var token = "";

            // Act
            var result = await _orderManagementLogic.AddOrderAsync(order, items, address, token);

            // Assert
            Assert.Equal((int)AddOrderFailureCode.MatchingDeliveryConditionNotFound, result.Item1);
        }

        [Fact]
        public async Task AddOrderAsync_TotalCostsBelowMinOrderValue()
        {
            // Arrange
            var order = _order;
            var items = _orderItems;
            var address = _address;
            var token = "";

            // Act
            var result = await _orderManagementLogic.AddOrderAsync(order, items, address, token);

            // Assert
            Assert.Equal((int)AddOrderFailureCode.TotalCostsBelowMinOrderValue, result.Item1);
        }
    }
}
