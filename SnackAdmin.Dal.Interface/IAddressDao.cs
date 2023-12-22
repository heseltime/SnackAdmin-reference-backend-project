using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Interface
{
    // Data Access Object
    public interface IAddressDao : IDataManipulation
    {
        Task<Address?> FindByIdAsync(int id);

        Task<Address?> FindByStreetAsync(string street);
    }
}
