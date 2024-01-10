using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IBusinessManagementLogic
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(string restaurantName);
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<Order> GetOrderByTokenAsync(string token);
        Task<int> UpdateOrderAsync(Order order);

        Task<IEnumerable<Menu>> GetAllMenuItemsAsync(string restaurantName);
        Task<Menu> GetMenuByIdAsync(int id);
        Task<int> UpdateMenuAsync(Menu menu);
        Task<int> AddMenuAsync(Menu menu);
        Task<int> DeleteMenuAsync(Menu menu);

        Task<IEnumerable<DeliveryCondition>> GetAllDeliveryConditionsAsync(string restaurantName);

        Task<DeliveryCondition> GetDeliveryConditionByIdAsync(int conditionId);
        Task<int> UpdateDeliveryConditionAsync(DeliveryCondition condition);
        Task<int> AddDeliveryConditionAsync(DeliveryCondition deliveryCondition);

        Task<int> DeleteDeliveryConditionAsync(DeliveryCondition condition);

        // for authentication and authorisation
        Task<Restaurant> GetRestaurantByname(string restaurantName);
    }
}
