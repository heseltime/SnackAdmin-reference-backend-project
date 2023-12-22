using Org.BouncyCastle.Asn1.X509;
using SnackAdmin.Dal.Ado;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Test
{
    public class TestOpeningHourDao
    {
        private IConfiguration _configuration;
        private readonly IOpeningHourDao _openingHourDao;

        public TestOpeningHourDao()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            this._openingHourDao = new AdoOpeningHourDao(DefaultConnectionFactory.FromConfiguration(_configuration, "SnackDbConnection"));
        }

        [Fact]
        public async void FindAllByOrderId_withExistingId_returnsOrderItems()
        {
            List<OpeningHour> openingHours = (List<OpeningHour>)await _openingHourDao.FindAllByRestaurantIdAsync(3);
            Assert.NotEmpty(openingHours);
        }

        [Fact]
        public async void FindAllByOrderId_withNonExistingId_returnsNoOrderItems()
        {
            List<OpeningHour> openingHours = (List<OpeningHour>)await _openingHourDao.FindAllByRestaurantIdAsync(1000000);
            Assert.Empty(openingHours);
        }


        // InsertAsync
        [Fact]
        public async void InsertAsync_withValidData_returnsTrue()
        {
            var hour = new OpeningHour(0, 1, Day.Holiday, 
                TimeSpan.Parse("19:00:00"), TimeSpan.Parse("20:00:00"));

            var id = await _openingHourDao.InsertAsync(hour);
            Assert.True(id != 0);

            // dirty clean up
            hour.Id = id;
            await _openingHourDao.DeleteAsync(hour);
        }

        [Fact]
        public async void InsertAsync_withInvalidData_throwsException()
        {
            var hour = new OpeningHour(0, 10000, Day.Holiday,
                TimeSpan.Parse("19:00:00"), TimeSpan.Parse("20:00:00"));

            await Assert.ThrowsAsync<Npgsql.PostgresException>(() => _openingHourDao.InsertAsync(hour));
        }

        // UpdateAsync
        [Fact]
        public async void UpdateAsync_withExistingData_returnsTrue()
        {
            var hour = new OpeningHour(0, 1, Day.Holiday,
                TimeSpan.Parse("19:00:00"), TimeSpan.Parse("20:00:00"));
            var id = await _openingHourDao.InsertAsync(hour);

            hour.OpenTime = TimeSpan.Parse("21:00:00");
            hour.Id = id;
            Assert.True(await _openingHourDao.UpdateAsync(hour));

            // dirty clean up
            await _openingHourDao.DeleteAsync(hour);
        }

        [Fact]
        public async void UpdateAsync_withNotExistingData_returnsFalse()
        {
            var hour = new OpeningHour(0, 10000, Day.Holiday,
                TimeSpan.Parse("19:00:00"), TimeSpan.Parse("20:00:00"));
            
            Assert.False(await _openingHourDao.UpdateAsync(hour));
        }

        // DeleteAsync
        [Fact]
        public async void DeleteAsync_withExistingData_returnsTrue()
        {
            var hour = new OpeningHour(0, 1, Day.Holiday,
                TimeSpan.Parse("19:00:00"), TimeSpan.Parse("20:00:00"));
            var id = await _openingHourDao.InsertAsync(hour);
            hour.Id = id;

            Assert.True(await _openingHourDao.DeleteAsync(hour));
        }

        [Fact]
        public async void DeleteAsync_withNotExistingData_returnsFalse()
        {
            var hour = new OpeningHour(0, 10000, Day.Holiday,
                TimeSpan.Parse("19:00:00"), TimeSpan.Parse("20:00:00"));

            Assert.False(await _openingHourDao.DeleteAsync(hour));
        }
    }
}