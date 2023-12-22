using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Interface
{
    // Data Access Object
    public interface IOrderItemDao : IDataManipulation
    {
        Task<IEnumerable<OrderItem>> FindAllByOrderIdAsync(Guid orderId);
    }
}
