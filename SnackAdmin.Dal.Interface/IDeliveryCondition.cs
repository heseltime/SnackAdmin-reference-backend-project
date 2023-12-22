using SnackAdmin.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Dal.Interface
{
    public interface IDeliveryCondition : IDataManipulation
    {
        Task<IEnumerable<DeliveryCondition>> FindAllByRestaurantIdAsync(int restaurantId);

        Task<DeliveryCondition?> FindByIdAsync(int id);
    }
}
