
using System.Numerics;
using System.Transactions;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic
{
    public class DalTesterAddress : IDalTester
    {
        private readonly IAddressDao addressDao;
        public DalTesterAddress(IAddressDao addressDao)
        {
            this.addressDao = addressDao;
        }

        public async Task<bool> ExecuteTestAsync()
        {
            Console.Out.WriteLine(addressDao.GetType());

            await TestFindByIdAsync();

            return true;
        }

        private async Task TestFindByIdAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### FindByIdAsync");
            Address? entity = await addressDao.FindByIdAsync(1);
            Console.WriteLine($"FindByIdAsync(1): {entity}");
            Console.WriteLine();
        }

    }
}
