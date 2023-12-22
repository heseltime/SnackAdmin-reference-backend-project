using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Test
{
    public class TestOrderDao
    {
        private IConfiguration _configuration;
        private readonly IOrderDao _orderDao;

        public TestOrderDao()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            this._orderDao = new AdoOrderDao(DefaultConnectionFactory.FromConfiguration(_configuration, "SnackDbConnection"));
        }

        [Fact]
        public async void FindAllByOrderId_withExistingId_returnsOrders()
        {
            List<Order> orders = (List<Order>)await _orderDao.FindAllByRestaurantIdAsync(1);
            Assert.NotEmpty(orders);
        }

        [Fact]
        public async void FindAllByOrderId_withNonExistingId_returnsNoOrders()
        {
            List<Order> orders = (List<Order>)await _orderDao.FindAllByRestaurantIdAsync(1000000);
            Assert.Empty(orders);
        }


        // InsertAsync
        [Fact]
        public async void InsertAsync_withValidData_returnsTrue()
        {
            var order = new Order(
                Guid.Empty, 1, 1, 1, DateTime.Now,
                48.52231449370366, 14.294476125935876, "bitte mit sauce",
                DeliveryStatus.OrderPlaced);

            var id = await _orderDao.InsertForGuidAsync(order);
            Assert.True(id != Guid.Empty);

            // dirty clean up
            order.Id = id;
            await _orderDao.DeleteAsync(order);
        }

        [Fact]
        public async void InsertAsync_withInvalidData_throwsPostgresException()
        {
            var order = new Order(
                Guid.Empty, 0, 1, 1, DateTime.Now, 
                48.52231449370366, 14.294476125935876, "bitte mit sauce",
                DeliveryStatus.OrderPlaced);

            await Assert.ThrowsAsync<Npgsql.PostgresException>(() => _orderDao.InsertForGuidAsync(order));
        }

        // UpdateAsync
        [Fact]
        public async void UpdateAsync_withExistingData_returnsTrue()
        {
            var order = new Order(
                Guid.Empty, 1, 1, 1, DateTime.Now,
                48.52231449370366, 14.294476125935876, "bitte mit sauce",
                DeliveryStatus.OrderPlaced);
            var id = await _orderDao.InsertForGuidAsync(order);

            order.FreeText = "updated";
            order.Id = id;
            Assert.True(await _orderDao.UpdateAsync(order));

            // dirty clean up
            await _orderDao.DeleteAsync(order);
        }

        [Fact]
        public async void UpdateAsync_withNotExistingData_returnsFalse()
        {
            var order = new Order(
                new Guid("00000000-0000-0000-0000-000000000000"), 1, 1, 1, DateTime.Now,
                48.52231449370366, 14.294476125935876, "bitte mit sauce",
                DeliveryStatus.OrderPlaced);

            Assert.False(await _orderDao.UpdateAsync(order));
        }

        // DeleteAsync
        [Fact]
        public async void DeleteAsync_withExistingData_returnsTrue()
        {
            var order = new Order(
                new Guid("00000000-0000-0000-0000-000000000000"), 1, 1, 1, DateTime.Now,
                48.52231449370366, 14.294476125935876, "bitte mit sauce",
                DeliveryStatus.OrderPlaced);
            var id = await _orderDao.InsertForGuidAsync(order);
            order.Id = id;

            Assert.True(await _orderDao.DeleteAsync(order));
        }

        [Fact]
        public async void DeleteAsync_withNotExistingData_returnsFalse()
        {
            var order = new Order(
                new Guid("00000000-0000-0000-0000-000000000000"), 1, 1, 1, DateTime.Now,
                48.52231449370366, 14.294476125935876, "bitte mit sauce",
                DeliveryStatus.OrderPlaced);

            Assert.False(await _orderDao.DeleteAsync(order));
        }


    }
}