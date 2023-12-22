using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Interface
{
    // Data Access Object
    public interface IRestaurantDao : IDataManipulation
    {
        Task<IEnumerable<Restaurant>> FindAllAsync();

        Task<Restaurant?> FindByIdAsync(int id);
        Task<Restaurant?> FindByNameAsync(string name);
        Task<Restaurant?> FindByApiKeyAsync(string apiKey);
    }
}
