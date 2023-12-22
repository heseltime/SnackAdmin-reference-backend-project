using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Interface
{
    // Data Access Object
    public interface IOrderDao : IDataManipulation
    {
        Task<IEnumerable<Order>> FindAllByRestaurantIdAsync(int restaurantId);

        Task<IEnumerable<Order>> FindAllByCustomerIdAsync(int customerId);

        Task<Order?> FindByIdAsync(Guid id);
        Task<Guid> InsertForGuidAsync(IEntity entity);

    }
}
