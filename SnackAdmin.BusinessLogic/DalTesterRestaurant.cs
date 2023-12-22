
using System.Numerics;
using System.Transactions;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic
{
    public class DalTesterRestaurant : IDalTester
    {
        private readonly IRestaurantDao restaurantDao;
        public DalTesterRestaurant(IRestaurantDao restaurantDao)
        {
            this.restaurantDao = restaurantDao;
        }

        public async Task<bool> ExecuteTestAsync()
        {
            Console.Out.WriteLine(restaurantDao.GetType());

            await TestFindByIdAsync();
            await TestFindAllAsync();
            //await TestFindAllAsyncTest();
            await TestInsertAsync();
            await TestUpdateAsync();

            return true;
        }

        public async Task TestFindByIdAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### FindByIdAsync");
            Restaurant? entity = await restaurantDao.FindByIdAsync(1);
            Console.WriteLine($"FindByIdAsync(1): {entity}");
            Console.WriteLine();
        }


        public async Task TestFindAllAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### FindByAllAsync");
            IEnumerable<Restaurant>? entities = await restaurantDao.FindAllAsync();
            foreach (Restaurant entity in entities)
            {
                Console.WriteLine($"{entity}");
            }
            Console.WriteLine();
        }

        //public async Task TestFindAllAsyncTest()
        //{
        //    Console.WriteLine("########################");
        //    Console.WriteLine("### FindByAllAsync");
        //    IEnumerable<Restaurant>? entities = await restaurantDao.FindAllAsync();

        //    Restaurant expectedRestaurant = new Restaurant(
        //        1, "Burgerei", 1,
        //        40.715, -74.009, "http://webhookBurgerei.url",
        //        "burgerei_image.jpg", "APIBurger123");

        //    Restaurant expectedRestaurant2 = await restaurantDao.FindByIdAsync(1);

        //    Console.WriteLine(expectedRestaurant);
        //    Console.WriteLine(expectedRestaurant2);
        //    Console.WriteLine(entities.First());
        //    Console.WriteLine(expectedRestaurant == entities.First());
        //    Console.WriteLine(expectedRestaurant2.Equals(entities.First()));


        //    Console.WriteLine();
        //}

        public async Task TestInsertAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### InsertAsync");

            IEntity restaurant = new Restaurant(
                1, "Latino Bar", 1,
                48.52231449370366, 14.294476125935876, "http://webhookLatino.url",
                null, "pub_is_very_good");

            Console.WriteLine(await restaurantDao.InsertAsync(restaurant));
            
            Console.WriteLine();
        }
        
        public async Task TestUpdateAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### UpdateAsync");

            string name = "UpdateTest";
            Restaurant restaurant = new Restaurant(
                0, name, 1,
                48.0, 14.0, "",
                null, "");

            await restaurantDao.InsertAsync(restaurant);
            restaurant.TitleImage = new byte[] {};
            restaurant.Id = await FindRestaurantIdByName(name);

            Console.WriteLine(await restaurantDao.UpdateAsync(restaurant));

            Console.WriteLine();
        }


        public async Task<int> FindRestaurantIdByName(string name)
        {
            IEnumerable<Restaurant>? entities = await restaurantDao.FindAllAsync();
            Restaurant restaurant = entities?.FirstOrDefault((e) => e.Name == name);

            return restaurant?.Id ?? 0;
        }

    }
}
