
using System.Numerics;
using System.Transactions;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic
{
    public class DalTesterOpeningHours : IDalTester
    {
        private readonly IOpeningHourDao openingHourDao;
        public DalTesterOpeningHours(IOpeningHourDao openingHourDao)
        {
            this.openingHourDao = openingHourDao;
        }

        public async Task<bool> ExecuteTestAsync()
        {
            Console.Out.WriteLine(openingHourDao.GetType());

            await TestFindByIdAsync();

            return true;
        }

        public async Task TestFindByIdAsync()
        {
            Console.WriteLine("########################");
            Console.WriteLine("### FindByIdAsync");
            OpeningHour? entity = await openingHourDao.FindByIdAsync(1);
            Console.WriteLine($"FindByIdAsync(1): {entity}");
            Console.WriteLine();
        }

    }
}
