using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Test
{

    public class TestOrderItemDao
    {
        private IConfiguration _configuration;
        private readonly IOrderItemDao _orderItemDao;

        public TestOrderItemDao()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            this._orderItemDao = new AdoOrderItemDao(DefaultConnectionFactory.FromConfiguration(_configuration, "SnackDbConnection"));
        }

        [Fact]
        public async void FindAllByOrderId_withExistingId_returnsOrderItems()
        {
            List<OrderItem> orderItems = (List<OrderItem>)await _orderItemDao.FindAllByOrderIdAsync(new Guid("881a9175-4be0-4207-b8ab-6386b4607610"));
            Assert.NotEmpty(orderItems);
        }

        [Fact]
        public async void FindAllByOrderId_withNonExistingId_returnsNoOrderItems()
        {
            List<OrderItem> orderItems = (List<OrderItem>)await _orderItemDao.FindAllByOrderIdAsync(Guid.Empty);
            Assert.Empty(orderItems);
        }

        // InsertAsync
        [Fact]
        public async void InsertAsync_withValidData_returnsTrue()
        {
            var orderItem = new OrderItem(new Guid("881a9175-4be0-4207-b8ab-6386b4607610"), 9,10);
            var id = await _orderItemDao.InsertAsync(orderItem);

            Assert.True(id > 0);

            // dirty clean up
            await _orderItemDao.DeleteAsync(orderItem);
        }

        [Fact]
        public async void InsertAsync_withInvalidData_throwsPostgresException()
        {
            var orderItem = new OrderItem(new Guid("881a9175-4be0-4207-b8ab-6386b4607610"), 0, 10);

            await Assert.ThrowsAsync<Npgsql.PostgresException>(() => _orderItemDao.InsertAsync(orderItem));

            // dirty clean up
            await _orderItemDao.DeleteAsync(orderItem);
        }

        //[Fact]
        //public async void InsertAsync_withDuplicatedData_throwsPostgresException()
        //{
        //    var orderItem = new OrderItem(5, 9, 10);
        //    var id = await _orderItemDao.InsertAsync(orderItem);

        //    await Assert.ThrowsAsync<Npgsql.PostgresException>(() => _orderItemDao.InsertAsync(orderItem));

        //    // dirty clean up
        //    await _orderItemDao.DeleteAsync(orderItem);
        //}

        // UpdateAsync
        [Fact]
        public async void UpdateAsync_withExistingData_returnsTrue()
        {
            var orderItem = new OrderItem(new Guid("881a9175-4be0-4207-b8ab-6386b4607610"), 9, 10);
            await _orderItemDao.InsertAsync(orderItem);
            
            Assert.True(await _orderItemDao.UpdateAsync(orderItem));

            // dirty clean up
            await _orderItemDao.DeleteAsync(orderItem);
        }

        [Fact]
        public async void UpdateAsync_withNotExistingData_returnsFalse()
        {
            var orderItem = new OrderItem(new Guid("881a9175-4be0-4207-b8ab-6386b4607610"), 10000, 10);

            Assert.False(await _orderItemDao.UpdateAsync(orderItem));
        }

        // DeleteAsync
        [Fact]
        public async void DeleteAsync_withExistingData_returnsTrue()
        {
            var orderItem = new OrderItem(new Guid("881a9175-4be0-4207-b8ab-6386b4607610"), 9, 10);
            await _orderItemDao.InsertAsync(orderItem);

            Assert.True(await _orderItemDao.DeleteAsync(orderItem));
        }

        [Fact]
        public async void DeleteAsync_withNotExistingData_returnsFalse()
        {
            var orderItem = new OrderItem(new Guid("881a9175-4be0-4207-b8ab-6386b4607610"), 10000, 10);

            Assert.False(await _orderItemDao.DeleteAsync(orderItem));
        }

    }
}