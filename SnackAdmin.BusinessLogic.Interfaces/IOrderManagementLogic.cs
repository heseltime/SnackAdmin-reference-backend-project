using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IOrderManagementLogic
    {
        Task<Order?> GetOrderAsync(Guid orderId);
        Task<IEnumerable<OrderItem>> GetOrderItemsAsync(Guid orderId);
        Task<Address?> GetAddressAsync(int addressId);
        Task<Restaurant?> GetRestaurantAsync(int restaurantId);
        Task<IEnumerable<Menu>> GetMenusAsync(int restaurantId);
        Task<Menu?> GetMenuByIdAsync(int menuId);
        Task<DeliveryCondition?> GetDeliveryConditionAsync(Restaurant restaurant, Order order);
        Task<(int, Guid)> AddOrderAsync(Order order, IEnumerable<OrderItem> items, Address address, string token);
        Task<bool> DeleteOrderAsync(Guid orderId);

    }
}
