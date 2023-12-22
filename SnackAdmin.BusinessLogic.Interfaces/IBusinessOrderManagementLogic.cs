using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IBusinessOrderManagementLogic
    {
        //Task<Order?> GetOrderByIdAsync(int orderId);
        Task<int> UpdateOrderAsync(Order order);
        //Task<IEnumerable<Order>> GetAllOrdersByRestaurantIdAsync(int restaurantId);
    }
}
