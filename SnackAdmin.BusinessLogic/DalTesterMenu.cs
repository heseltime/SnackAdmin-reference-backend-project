
using System.Numerics;
using System.Transactions;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic
{
    public class DalTesterMenu : IDalTester
    {
        private readonly IMenuDao dao;
        public DalTesterMenu(IMenuDao dao)
        {
            this.dao = dao;
        }

        public async Task<bool> ExecuteTestAsync()
        {
            Console.Out.WriteLine(dao.GetType());

            await TestFindByIdAsync();
            await TestDeleteAsync();

            return true;
        }

        private async Task TestFindByIdAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### FindByIdAsync");
            IEntity? entity = await dao.FindByIdAsync(1);
            Console.WriteLine($"FindByIdAsync(1): {entity}");
            Console.WriteLine();
        }
        private async Task TestDeleteAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### DeleteAsync");
            var menu = new Menu(
                0, 1, "cat", "item", "desc", (decimal)9.99);
            var id = await dao.InsertAsync(menu);
            menu.Id = id;

            Console.WriteLine($"DeleteAsync: {menu} = {await dao.DeleteAsync(menu)}");
            Console.WriteLine();
        }

    }
}
