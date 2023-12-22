using Dal.Common;
using Microsoft.Extensions.Configuration;
using SnackAdmin.BusinessLogic;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Ado;
using SnackAdmin.Dal.Interface;

Console.WriteLine();

var configuration =
    new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

IConnectionFactory connectionFactory = DefaultConnectionFactory.FromConfiguration(configuration, "SnackDbConnection");

await Test(
    new DalTesterAddress(
        new AdoAddressDao(connectionFactory)));

await Test(
    new DalTesterMenu(
        new AdoMenuDao(connectionFactory)));

await Test(
    new DalTesterRestaurant(
        new AdoRestaurantDao(connectionFactory)));

await Test(
    new DalTesterOrder(
        new AdoOrderDao(connectionFactory)));

await Test(
    new DalTesterOpeningHours(
        new AdoOpeningHourDao(connectionFactory)));

async Task Test(IDalTester tester)
{
    await tester.ExecuteTestAsync();
}



//await Test(new SimplePersonDao());

//await Test(new AdoPersonDao(DefaultConnectionFactory.FromConfiguration(configuration, "SnackDbConnection")));

//async Task Test(IPersonDao personDao)
//{
//    Console.WriteLine(personDao.GetType());

//    var tester = new DalTester(personDao);

//    await tester.TestFindAllAsync();
//    await tester.TestFindByIdAsync();
//    await tester.TestUpdateAsync();
//    await tester.TestTransactionsAsync();
//}