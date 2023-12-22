using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnackAdmin.BusinessLogic.Common;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Domain;
using SnackAdmin.Dal.Interface;

namespace SnackAdmin.BusinessLogic
{
    public class RestaurantManagementLogic : IRestaurantManagementLogic
    {
        private readonly IRestaurantDao _restaurantDao;
        private readonly IDeliveryCondition _deliveryConditionDao;
        private readonly IAddressDao _addressDao;
        private readonly IOpeningHourDao _openingHourDao;

        public RestaurantManagementLogic(IRestaurantDao restaurantDao, IAddressDao addressDao, IDeliveryCondition deliveryConditionDao, IOpeningHourDao openingHourDao)
        {
            _restaurantDao = restaurantDao;
            _addressDao = addressDao;
            _deliveryConditionDao = deliveryConditionDao;
            _openingHourDao = openingHourDao;
        }
        
        public async Task<IEnumerable<Restaurant>> GetRestaurantsAsync(double gpsLat, double gpsLong, int radius)
        {
            IEnumerable<Restaurant> restaurants = await _restaurantDao.FindAllAsync();

            restaurants = restaurants.
                Where((r) => 
                    Calculations.DistanceBetweenGpsCoordinates(
                        r.GpsLat, r.GpsLong, gpsLat, gpsLong) < radius
                    );
            return restaurants;
        }

        public async Task<Address?> GetAddressByIdAsync(int addressId)
        {
            return await _addressDao.FindByIdAsync(addressId);
        }

        public async Task<OpeningHour?> GetActualOpeningHourByRestaurantIdAsync(int restaurantId)
        {
            var hours = await _openingHourDao.FindAllByRestaurantIdAsync(restaurantId);
            return hours.FirstOrDefault(h => (int)h.Day == DateTime.Now.Day);
        }

        public async Task<DeliveryCondition?> GetDeliveryConditionForDistance(double gpsLat, double gpsLong, Restaurant restaurant)
        {
            var distance =
                Calculations.DistanceBetweenGpsCoordinates(gpsLat, gpsLong, restaurant.GpsLat, restaurant.GpsLong);
            var conditions = await _deliveryConditionDao.FindAllByRestaurantIdAsync(restaurant.Id);
            var orderedConditions = conditions.OrderBy(x => x.Distance);

            return orderedConditions.FirstOrDefault(dc => dc.Distance > distance);
        }


    }
}
