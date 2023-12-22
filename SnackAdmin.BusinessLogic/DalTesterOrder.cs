
using System.Numerics;
using System.Transactions;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic
{
    public class DalTesterOrder : IDalTester
    {
        private readonly IOrderDao orderDao;
        public DalTesterOrder(IOrderDao orderDao)
        {
            this.orderDao = orderDao;
        }

        public async Task<bool> ExecuteTestAsync()
        {
            Console.Out.WriteLine(orderDao.GetType());

            await TestFindByIdAsync();
            await TestDeleteAsync();

            return true;
        }

        public async Task TestFindByIdAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### FindByIdAsync");
            Order? entity = await orderDao.FindByIdAsync(new Guid("881a9175 - 4be0 - 4207 - b8ab - 6386b4607610"));
            Console.WriteLine($"FindByIdAsync(1): {entity}");
            Console.WriteLine();
        }
        
        public async Task TestDeleteAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### DeleteAsync ");
            var order = new Order(
                new Guid("991a9175 - 4be0 - 4207 - b8ab - 6386b4607610"), 1, 1, 1, DateTime.Now,
                48.52231449370366, 14.294476125935876, "bitte mit sauce",
                DeliveryStatus.OrderPlaced);

            await orderDao.InsertAsync(order);


            order.Id = await orderDao.InsertForGuidAsync(order);
            Console.WriteLine($"id = {order.Id}");

            Console.WriteLine(await orderDao.DeleteAsync(order));
            Console.WriteLine();
        }
    }
}
