using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Test
{
    public class TestCustomerDao
    {
        private IConfiguration _configuration;
        private readonly ICustomerDao _customerDao;

        public TestCustomerDao()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            this._customerDao = new AdoCustomerDao(DefaultConnectionFactory.FromConfiguration(_configuration, "SnackDbConnection"));
        }

        [Fact]
        public async void FindById_withExistingId_returnsCustomerId()
        {
            Customer? customer = await _customerDao.FindByIdAsync(1);
            Assert.NotNull(customer);
        }

        [Fact]
        public async void FindById_withNonExistingId_returnsNoCustomerId()
        {
            Customer? customer = await _customerDao.FindByIdAsync(1000000);
            Assert.Null(customer);
        }


        // InsertAsync
        [Fact]
        public async void InsertAsync_withValidData_returnsTrue()
        {
            Customer customer = new Customer(
                0,
                "ilkinator", 
                "c89b4c5a6f77b4c0c3e7bc425a663bbfdbaf6d4c12d3e07130328e713f3004f4", 
                "random_salt_1");
            var id = await _customerDao.InsertAsync(customer);

            Assert.True(id > 0);

            customer.Id = id;
            await _customerDao.DeleteAsync(customer);
        }

        [Fact]
        public async void InsertAsync_withInvalidData_returnsFalse()
        {
            Restaurant restaurant = new Restaurant( // test entering wrong entity into db
                0, "Latino Bar", 1,
                48.52231449370366, 14.294476125935876, "http://webhookLatino.url",
                null, "pub_is_very_good");

            await Assert.ThrowsAsync<NotSupportedException>(() => _customerDao.InsertAsync(restaurant));
        }

        // UpdateAsync
        [Fact]
        public async void UpdateAsync_withExistingData_returnsTrue()
        {
            Customer customer = new Customer(
                0,
                "ilkinator",
                "c89b4c5a6f77b4c0c3e7bc425a663bbfdbaf6d4c12d3e07130328e713f3004f4",
                "random_salt_1");
            var id = await _customerDao.InsertAsync(customer);

            customer.PasswordHash = "2f0c86b8b0830e5f0cc39aae74c5eaf0871fc8f6b4c1c4d8eb7a71cc89c7d149";
            customer.Id = id;
            Assert.True(await _customerDao.UpdateAsync(customer));

            // dirty clean up
            await _customerDao.DeleteAsync(customer);
        }

        [Fact]
        public async void UpdateAsync_withNotExistingData_returnsFalse()
        {
            Customer customer = new Customer(
                0,
                "ilkinator",
                "c89b4c5a6f77b4c0c3e7bc425a663bbfdbaf6d4c12d3e07130328e713f3004f4",
                "random_salt_1");

            Assert.False(await _customerDao.UpdateAsync(customer));
        }

        // DeleteAsync
        [Fact]
        public async void DeleteAsync_withExistingData_returnsTrue()
        {
            Customer customer = new Customer(
                0,
                "ilkinator",
                "c89b4c5a6f77b4c0c3e7bc425a663bbfdbaf6d4c12d3e07130328e713f3004f4",
                "random_salt_1");
            var id = await _customerDao.InsertAsync(customer);

            // dirty clean up
            customer.Id = id;
            Assert.True(await _customerDao.DeleteAsync(customer));
        }

        [Fact]
        public async void DeleteAsync_withNotExistingData_returnsFalse()
        {
            Customer customer = new Customer(
                0,
                "ilkinator",
                "c89b4c5a6f77b4c0c3e7bc425a663bbfdbaf6d4c12d3e07130328e713f3004f4",
                "random_salt_1");

            Assert.False(await _customerDao.DeleteAsync(customer));
        }
    }

}