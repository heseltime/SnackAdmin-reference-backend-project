using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Test
{
    public class TestMenuDao
    {
        private IConfiguration _configuration;
        private readonly IMenuDao _menuDao;

        public TestMenuDao()
        {
            this._configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            this._menuDao = new AdoMenuDao(DefaultConnectionFactory.FromConfiguration(_configuration, "SnackDbConnection"));
        }

        [Fact]
        public async void FindAllByRestaurantId_withExistingId_returnsMenus()
        {
            List<Menu> menus = (List<Menu>)await _menuDao.FindAllByRestaurantIdAsync(1);
            Assert.NotEmpty(menus);
        }

        [Fact]
        public async void FindAllByRestaurantId_withNonExistingId_returnsNoMenus()
        {
            List<Menu> menus = (List<Menu>)await _menuDao.FindAllByRestaurantIdAsync(1000000);
            Assert.Empty(menus);
        }


        // InsertAsync
        [Fact]
        public async void InsertAsync_withValidData_returnsTrue()
        {
            var menu = new Menu(
                0, 1, "cat", "item", "desc", (decimal)9.99);

            var id = await _menuDao.InsertAsync(menu);
            Assert.True(id != 0);

            // dirty clean up
            menu.Id = id;
            await _menuDao.DeleteAsync(menu);
        }

        [Fact]
        public async void InsertAsync_withInvalidData_throwsException()
        {
            var menu = new Menu(
                0, 10000, "cat", "item", "desc", (decimal)9.99);

            await Assert.ThrowsAsync<Npgsql.PostgresException>(() => _menuDao.InsertAsync(menu));

        }

        // UpdateAsync
        [Fact]
        public async void UpdateAsync_withExistingData_returnsTrue()
        {
            var menu = new Menu(
                0, 1, "cat", "item", "desc", (decimal)9.99);
            var id = await _menuDao.InsertAsync(menu);

            menu.Category = "updated";
            menu.Id = id;
            Assert.True(await _menuDao.UpdateAsync(menu));

            // dirty clean up
            await _menuDao.DeleteAsync(menu);
        }

        [Fact]
        public async void UpdateAsync_withNotExistingData_returnsFalse()
        {
            var menu = new Menu(
                0, 10000, "cat", "item", "desc", (decimal)9.99);

            Assert.False(await _menuDao.UpdateAsync(menu));
        }

        // DeleteAsync
        [Fact]
        public async void DeleteAsync_withExistingData_returnsTrue()
        {
            var menu = new Menu(
                0, 1, "cat", "item", "desc", (decimal)9.99);
            var id = await _menuDao.InsertAsync(menu);
            menu.Id = id;

            Assert.True(await _menuDao.DeleteAsync(menu));
        }

        [Fact]
        public async void DeleteAsync_withNotExistingData_returnsFalse()
        {
            var menu = new Menu(
                0, 10000, "cat", "item", "desc", (decimal)9.99);
            

            Assert.False(await _menuDao.DeleteAsync(menu));
        }
    }
}