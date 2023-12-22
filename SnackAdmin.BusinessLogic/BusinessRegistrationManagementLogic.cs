using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SnackAdmin.BusinessLogic.Common;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic
{
    public static class ApiKeyGenerator
    {
        private static readonly Random random = new Random();

        public static string GenerateApiKey(int size = 32)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, size)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class BusinessRegistrationManagementLogic : IBusinessRegistrationManagementLogic
    {
        private readonly IAddressDao _addressDao;
        private readonly IRestaurantDao _restaurantDao;
        private readonly IMenuDao _menuDao;
        private readonly IDeliveryCondition _deliveryConditionDao;
        private readonly IOpeningHourDao _hourDao;
        private readonly IOrderDao _orderDao;

        public BusinessRegistrationManagementLogic(
            IRestaurantDao restaurantDao, 
            IAddressDao addressDao, 
            IMenuDao menuDao, 
            IDeliveryCondition deliveryConditionDao, 
            IOpeningHourDao hourDao,
            IOrderDao orderDao)
        {
            _restaurantDao = restaurantDao;
            _addressDao = addressDao;
            _menuDao = menuDao;
            _deliveryConditionDao = deliveryConditionDao;
            _hourDao = hourDao;
            _orderDao = orderDao;
        }

        public async Task<(int, Restaurant?)> AddRestaurantAsync(RestaurantForCreation restaurantForCreation)
        {
            // Conflict Checks
            var existingRestaurant = await _restaurantDao.FindByNameAsync(restaurantForCreation.Name);
            if (existingRestaurant != null)
            {
                return ((int)AddRestaurantFailureCode.Conflict, null);
            }

            // Bad Request Checks
            if (string.IsNullOrWhiteSpace(restaurantForCreation.Name))
            {
                return ((int)AddRestaurantFailureCode.BadRequest, null);
            }

            var address = restaurantForCreation.NewAddress;
            if (address == null || string.IsNullOrWhiteSpace(address.Street) ||
                string.IsNullOrWhiteSpace(address.City) ||
                string.IsNullOrWhiteSpace(address.State) ||
                string.IsNullOrWhiteSpace(address.PostalCode) ||
                string.IsNullOrWhiteSpace(address.Country))
            {
                return ((int)AddRestaurantFailureCode.BadRequest, null);
            }

            if (restaurantForCreation.GpsLat < -90 || restaurantForCreation.GpsLat > 90 ||
                restaurantForCreation.GpsLong < -180 || restaurantForCreation.GpsLong > 180)
            {
                return ((int)AddRestaurantFailureCode.BadRequest, null);
            }

            if (!string.IsNullOrWhiteSpace(restaurantForCreation.WebHookUrl) &&
                !Uri.TryCreate(restaurantForCreation.WebHookUrl, UriKind.Absolute, out var uriResult))
            {
                return ((int)AddRestaurantFailureCode.BadRequest, null);
            }

            foreach (var openingHour in restaurantForCreation.OpeningHours)
            {
                // Additional checks like open time must be less than close time
                if (openingHour.OpenTime >= openingHour.CloseTime)
                {
                    return ((int)AddRestaurantFailureCode.BadRequest, null);
                }
            }

            // Make restaurant from restaurantForCreation
            var restaurant = new Restaurant();
            restaurant.Name = restaurantForCreation.Name;

            // add address to get ID
            var addressId = await _addressDao.InsertAsync(address);

            restaurant.AddressId = addressId;

            restaurant.GpsLat = restaurantForCreation.GpsLat;
            restaurant.GpsLong = restaurantForCreation.GpsLong;

            restaurant.WebHookUrl = restaurantForCreation.WebHookUrl;

            restaurant.TitleImage = restaurantForCreation.TitleImage;

            // generate API key
            restaurant.ApiKey = ApiKeyGenerator.GenerateApiKey();

            int restaurantId = await _restaurantDao.InsertAsync(restaurant);

            // add the relevant opening hours using the new restaurant id
            foreach (var openingHour in restaurantForCreation.OpeningHours)
            {
                openingHour.RestaurantId = restaurantId;
                await _hourDao.InsertAsync(openingHour);
            }

            // if ok then 1, else 0 or  -1, would need better handling from InsertAsync
            return ((int)AddRestaurantFailureCode.Ok, restaurant);
        }

        public enum AddRestaurantFailureCode
        {
            Default = -4,
            ServiceUnavailable = -3,
            BadRequest = -2,
            Conflict = -1,
            Ok = 0
        }
    }
}
