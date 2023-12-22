using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Interface
{
    // Data Access Object
    public interface IOpeningHourDao : IDataManipulation
    {
        Task<IEnumerable<OpeningHour>> FindAllByRestaurantIdAsync(int restaurantId);

        Task<OpeningHour?> FindByIdAsync(int id);
    }
}
