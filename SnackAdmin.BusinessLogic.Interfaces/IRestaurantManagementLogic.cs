using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IRestaurantManagementLogic
    {
        Task<IEnumerable<Restaurant>> GetRestaurantsAsync(double gpsLat, double gpsLong, int radius);
        Task<Address?> GetAddressByIdAsync(int addressId);
        Task<OpeningHour?> GetActualOpeningHourByRestaurantIdAsync(int restaurantId);
        Task<DeliveryCondition?> GetDeliveryConditionForDistance(double gpsLat, double gpsLong, Restaurant restaurant);
    }
}
