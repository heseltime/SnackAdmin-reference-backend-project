using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Interface
{
    // Data Access Object
    public interface ICustomerDao : IDataManipulation
    {
        Task<Customer?> FindByIdAsync(int id);
        Task<Customer?> FindByUserNameAsync(string userName);
    }
}
