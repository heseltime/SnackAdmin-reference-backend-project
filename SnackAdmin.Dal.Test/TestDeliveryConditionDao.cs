using Org.BouncyCastle.Asn1.X509;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Test
{
    public class TestDeliveryConditionDao
    {
        private IConfiguration _configuration;
        private readonly IDeliveryCondition _deliveryConditionDao;

        public TestDeliveryConditionDao()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            this._deliveryConditionDao = new AdoDeliveryConditionDao(DefaultConnectionFactory.FromConfiguration(_configuration, "SnackDbConnection"));
        }

        [Fact]
        public async void FindAllByRestaurantIdAsync_withExistingId_returnsDeliveryConditions()
        {
            List<DeliveryCondition> deliveryConditions = (List<DeliveryCondition>)await _deliveryConditionDao.FindAllByRestaurantIdAsync(1);
            Assert.NotEmpty(deliveryConditions);
        }

        [Fact]
        public async void FindAllByRestaurantId_withNonExistingId_returnsNoDeliveryConditions()
        {
            List<DeliveryCondition> deliveryConditions = (List<DeliveryCondition>)await _deliveryConditionDao.FindAllByRestaurantIdAsync(1000000);
            Assert.Empty(deliveryConditions);
        }


        // InsertAsync
        [Fact]
        public async void InsertAsync_withValidData_returnsTrue()
        {
            var condition = new DeliveryCondition(0,1,10,20,5);

            var id = await _deliveryConditionDao.InsertAsync(condition);
            Assert.True(id != 0);

            // dirty clean up
            condition.Id = id;
            await _deliveryConditionDao.DeleteAsync(condition);
        }

        [Fact]
        public async void InsertAsync_withInvalidData_returnsFalse()
        {
            var condition = new DeliveryCondition(0, 10000, 10, 20, 5);

            await Assert.ThrowsAsync<Npgsql.PostgresException>(() => _deliveryConditionDao.InsertAsync(condition));
        }

        // UpdateAsync
        [Fact]
        public async void UpdateAsync_withExistingData_returnsTrue()
        {
            var condition = new DeliveryCondition(0, 1, 10, 20, 5);
            var id = await _deliveryConditionDao.InsertAsync(condition);

            condition.Distance = 50;
            condition.Id = id;
            Assert.True(await _deliveryConditionDao.UpdateAsync(condition));

            // dirty clean up
            await _deliveryConditionDao.DeleteAsync(condition);
        }

        [Fact]
        public async void UpdateAsync_withNotExistingData_returnsFalse()
        {
            var condition = new DeliveryCondition(0, 10000, 10, 20, 5);

            Assert.False(await _deliveryConditionDao.UpdateAsync(condition));
        }

        // DeleteAsync
        [Fact]
        public async void DeleteAsync_withExistingData_returnsTrue()
        {
            var condition = new DeliveryCondition(0, 1, 10, 20, 5);

            var id = await _deliveryConditionDao.InsertAsync(condition);
            condition.Id = id;

            Assert.True(await _deliveryConditionDao.DeleteAsync(condition));
        }

        [Fact]
        public async void DeleteAsync_withNotExistingData_returnsFalse()
        {
            var condition = new DeliveryCondition(0, 10000, 10, 20, 5);
            
            Assert.False(await _deliveryConditionDao.DeleteAsync(condition));
        }
    }

}