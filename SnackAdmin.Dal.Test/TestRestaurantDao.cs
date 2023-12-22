

using SnackAdmin.Domain;
using System.Collections.Generic;

namespace SnackAdmin.Dal.Test
{
    public class TestRestaurantDao
    {
        private IConfiguration _configuration;
        private readonly IRestaurantDao _restaurantDao;

        public TestRestaurantDao()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            this._restaurantDao =
                new AdoRestaurantDao(DefaultConnectionFactory.FromConfiguration(_configuration, "SnackDbConnection"));
        }

        // FindByIdAsync
        [Fact]
        public async void FindRestaurantById_withExistingId_returnsRestaurant()
        {
            Restaurant? entity = await _restaurantDao.FindByIdAsync(1);
            Assert.Equal(1, entity?.Id);
        }

        [Fact]
        public async void FindRestaurantById_withNotExistingId_returnsNoRestaurant()
        {
            Restaurant? entity = await _restaurantDao.FindByIdAsync(1000000);
            Assert.Null(entity);
        }

        // FindByApiKeyAsync
        [Fact]
        public async void FindRestaurantByApiKey_withExistingApiKey_returnsRestaurant()
        {
            Restaurant? entity = await _restaurantDao.FindByApiKeyAsync("APIBurger123");
            Assert.Equal(1, entity?.Id);
        }

        [Fact]
        public async void FindRestaurantByApiKey_withNotExistingApiKey_returnsNoRestaurant()
        {
            Restaurant? entity = await _restaurantDao.FindByApiKeyAsync("test");
            Assert.Null(entity);
        }

        // FindAllAsync
        [Fact]
        public async void FindAllRestaurants_returnsAllRestaurant()
        {
            // Arrange
            IEnumerable<Restaurant> expectedRestaurants = new List<Restaurant>()
            {
                new Restaurant(
                    1, "Burgerei", 1,
                    40.715, -74.009, "http://webhookBurgerei.url",
                    new byte[]{}, "APIBurger123"),
                new Restaurant(
                    2, "Salz und Pfeffer", 2,
                    40.716, -74.01, "http://webhookSalzundPfeffer.url",
                    new byte[]{}, "APISalz321"),
                new Restaurant(
                    3, "Campina", 3,
                    40.717, -74.011, "http://webhookCampina.url",
                    new byte[]{}, "APICampina789")
            };

            // Act
            IEnumerable<Restaurant>? entities = (List<Restaurant>)await _restaurantDao.FindAllAsync();
            
            // Assert
            foreach (var entity in entities)
            {
                Assert.Contains(entity, entities);
            }
        }

        // InsertAsync
        [Fact]
        public async void InsertAsync_withValidData_returnsTrue()
        {
            Restaurant restaurant = new Restaurant(
                0, "Latino Bar", 1,
                48.52231449370366, 14.294476125935876, "http://webhookLatino.url",
                new byte[] { }, "pub_is_very_good");

            var id = await _restaurantDao.InsertAsync(restaurant);
            Assert.True(id > 0);

            // dirty clean up
            restaurant.Id = id;
            await _restaurantDao.DeleteAsync(restaurant);
        }

        [Fact]
        public async void InsertAsync_withInvalidData_throwsInvalidOperationException()
        {
            Restaurant restaurant = new Restaurant(
                0, "Latino Bar", 1,
                48.52231449370366, 14.294476125935876, "http://webhookLatino.url",
                new byte[] { }, null);
            
            await Assert.ThrowsAsync<InvalidOperationException>(() => _restaurantDao.InsertAsync(restaurant));
        }
        
        // UpdateAsync
        [Fact]
        public async void UpdateAsync_withExistingRestaurant_returnsTrue()
        {
            string name = "UpdateTest";
            Restaurant restaurant = new Restaurant(
                0, name, 1,
                48.0, 14.0, "",
                new byte[] { }, "");

            var id = await _restaurantDao.InsertAsync(restaurant);
            restaurant.TitleImage = new byte[] {};
            restaurant.Id = id;

            Assert.True(await _restaurantDao.UpdateAsync(restaurant));

            // dirty clean up
            await _restaurantDao.DeleteAsync(restaurant);
        }

        [Fact]
        public async void UpdateAsync_withNotExistingRestaurant_returnsFalse()
        {
            string name = "UpdateTest";
            Restaurant restaurant = new Restaurant(
                0, name, 1,
                48.0, 14.0, "",
                new byte[] { }, "");

            Assert.False(await _restaurantDao.UpdateAsync(restaurant));
        }

        // DeleteAsync
        [Fact]
        public async void DeleteAsync_withExistingRestaurant_returnsTrue()
        {
            string name = "DeleteTest";
            Restaurant restaurant = new Restaurant(
                0, name, 1,
                48.0, 14.0, "",
                new byte[] { }, "");

            var id = await _restaurantDao.InsertAsync(restaurant);
            restaurant.Id = id;

            Assert.True(await _restaurantDao.DeleteAsync(restaurant));
        }

        [Fact]
        public async void DeleteAsync_withNotExistingRestaurant_returnsFalse()
        {
            Restaurant restaurant = new Restaurant(
                0, "dummy", 1,
                48.0, 14.0, "",
                new byte[] { }, "");

            Assert.False(await _restaurantDao.DeleteAsync(restaurant));
        }


        // Helper methods
        //public async Task<int> FindRestaurantIdByName(string name)
        //{
        //    IEnumerable<Restaurant>? entities = await _restaurantDao.FindAllAsync();
        //    Restaurant restaurant = entities?.FirstOrDefault((e) => e.Name == name);

        //    return restaurant?.Id ?? 0;
        //}

    }
}