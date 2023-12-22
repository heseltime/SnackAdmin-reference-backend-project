using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Interface
{
    // Data Access Object
    public interface IMenuDao : IDataManipulation
    {
        Task<IEnumerable<Menu>> FindAllByRestaurantIdAsync(int id);

        Task<Menu?> FindByIdAsync(int id);
    }
}
