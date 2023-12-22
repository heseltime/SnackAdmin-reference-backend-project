
using System.Numerics;
using System.Transactions;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic
{
    [Obsolete]
    public class DalTester
    {
        //private readonly IPersonDao personDao;
        //public DalTester(IPersonDao personDao)
        //{
        //    this.personDao = personDao;
        //}

        //public async Task TestFindAllAsync()
        //{
        //    Console.WriteLine("FindAllAsync");
        //    foreach (var person in await this.personDao.FindAllAsync())
        //    {
        //        Console.WriteLine(
        //            $"{person.Id,5} | {person.FirstName, -10} | {person.LastName,-15} | {person.DateOfBirth,10:yyyy-MM-dd} ");
        //    }
        //    Console.WriteLine();
        //}

        //public async Task TestFindByIdAsync()
        //{
        //    Console.WriteLine("########################");
        //    Console.WriteLine("### FindByIdAsync");
        //    Person? person = await personDao.FindByIdAsync(1);
        //    Console.WriteLine($"FindByIdAsync(1): {person}");
        //    Console.WriteLine();
        //}

        //public async Task TestUpdateAsync()
        //{
        //    Console.WriteLine("########################");
        //    Console.WriteLine("### Test UpdateAsync ###");
        //    Person? person = await personDao.FindByIdAsync(1);
        //    Console.WriteLine("Before update: " + person);

        //    if (person is null) return;

        //    person.DateOfBirth = person.DateOfBirth.AddYears(1);
        //    await personDao.UpdateAsync(person);

        //    person= await personDao.FindByIdAsync(1);
        //    Console.WriteLine("After update: " + person);
        //    Console.WriteLine();
        //}

        //public async Task TestTransactionsAsync()
        //{
        //    Console.WriteLine("########################");
        //    Console.WriteLine("### Test Transaction ###");
        //    try
        //    {
        //        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {

        //            await personDao.UpdateAsync(new Person(2, "Before", "Exception", DateTime.Now));

        //            //throw new Exception(); // uncomment this line to rollback transaction

        //            await personDao.UpdateAsync(new Person(2, "After", "Exception", DateTime.Now));

        //            scope.Complete();
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }

        //    Console.WriteLine("Transaction Completed");
        //    Console.WriteLine();
        //}
    }
}
