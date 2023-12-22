using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Test
{
    public class TestAddressDao
    {
        private IConfiguration _configuration;
        private readonly IAddressDao _addressDao;

        public TestAddressDao()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            this._addressDao = new AdoAddressDao(DefaultConnectionFactory.FromConfiguration(_configuration, "SnackDbConnection"));
        }

        [Fact]
        public async void FindById_withExistingId_returnsAddressId()
        {
            Address? address = await _addressDao.FindByIdAsync(1);
            Assert.NotNull(address);
        }

        [Fact]
        public async void FindById_withNonExistingId_returnsNoAddressId()
        {
            Address? address = await _addressDao.FindByIdAsync(1000000);
            Assert.Null(address);
        }


        // InsertAsync
        [Fact]
        public async void InsertAsync_withValidData_returnsTrue()
        {
            Address address = new Address(
                100,"808 Test Street", "4813", "Altmuenster",
                "Upper Austria", "Austria");
            var id = await _addressDao.InsertAsync(address);

            Assert.True(id > 0);

            address.Id = id;
            await _addressDao.DeleteAsync(address);
        }

        [Fact]
        public async void InsertAsync_withInvalidData_returnsFalse()
        {
            Restaurant restaurant = new Restaurant( // test entering wrong entity into db
                0, "Latino Bar", 1,
                48.52231449370366, 14.294476125935876, "http://webhookLatino.url",
                null, "pub_is_very_good");

            await Assert.ThrowsAsync<NotSupportedException>(() => _addressDao.InsertAsync(restaurant));
        }

        // UpdateAsync
        [Fact]
        public async void UpdateAsync_withExistingData_returnsTrue()
        {
            Address address = new Address(
                1, "123 Main Street", "12345", "Anytown",
                "CA", "USB");
            // ('123 Main Street', '12345', 'Anytown', 'CA', 'USA'),

            Assert.True(await _addressDao.UpdateAsync(address));

            // return to original status
            address = new Address(
                1, "123 Main Street", "12345", "Anytown",
                "CA", "USA");
            // ('123 Main Street', '12345', 'Anytown', 'CA', 'USA'),

            await _addressDao.UpdateAsync(address);
        }

        [Fact]
        public async void UpdateAsync_withNotExistingData_returnsFalse()
        {
            Address address = new Address(
                100000, "123 Main Street", "12345", "Anytown",
                "CA", "USA");
            // ('123 Main Street', '12345', 'Anytown', 'CA', 'USA'),

            Assert.False(await _addressDao.UpdateAsync(address));
        }

        // DeleteAsync
        [Fact]
        public async void DeleteAsync_withExistingData_returnsTrue()
        {
            Address address = new Address(
                100000, "Delete", "12345", "Anytown",
                "CA", "USA");
            var id = await _addressDao.InsertAsync(address);

            address.Id = id;
            Assert.True(await _addressDao.DeleteAsync(address));
        }

        [Fact]
        public async void DeleteAsync_withNotExistingData_returnsFalse()
        {
            Address address = new Address(
                0, "123 Main Street", "12345", "Anytown",
                "CA", "USB");

            Assert.False(await _addressDao.DeleteAsync(address));
        }
    }

}