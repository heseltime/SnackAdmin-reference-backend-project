using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class OrderManagementLogic : IOrderManagementLogic
    {
        private readonly IAddressDao _addressDao;
        private readonly IRestaurantDao _restaurantDao;
        private readonly IOrderItemDao _orderItemDao;
        private readonly IOrderDao _orderDao;
        private readonly IMenuDao _menuDao;
        private readonly IDeliveryCondition _deliveryConditionDao;
        private readonly IOpeningHourDao _hourDao;

        public OrderManagementLogic(
            IOrderItemDao orderItemDao, 
            IOrderDao orderDao, 
            IRestaurantDao restaurantDao, 
            IAddressDao addressDao, 
            IMenuDao menuDao, 
            IDeliveryCondition deliveryConditionDao, 
            IOpeningHourDao hourDao)
        {
            _orderItemDao = orderItemDao;
            _orderDao = orderDao;
            _restaurantDao = restaurantDao;
            _addressDao = addressDao;
            _menuDao = menuDao;
            _deliveryConditionDao = deliveryConditionDao;
            _hourDao = hourDao;
        }

        public async Task<Menu?> GetMenuByIdAsync(int menuId)
        {
            return await _menuDao.FindByIdAsync(menuId);
        }

        public async Task<Order?> GetOrderAsync(Guid orderId)
        {
            return await _orderDao.FindByIdAsync(orderId);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsAsync(Guid orderId)
        {
            return await _orderItemDao.FindAllByOrderIdAsync(orderId);
        }

        public async Task<Address?> GetAddressAsync(int addressId)
        {
            return await _addressDao.FindByIdAsync(addressId);
        }

        public async Task<Restaurant?> GetRestaurantAsync(int restaurantId)
        {
            return await _restaurantDao.FindByIdAsync(restaurantId);
        }

        public async Task<IEnumerable<Menu>> GetMenusAsync(int restaurantId)
        {
            return await _menuDao.FindAllByRestaurantIdAsync(restaurantId);
        }

        public async Task<DeliveryCondition?> GetDeliveryConditionAsync(Restaurant restaurant, Order order)
        {
            return await GetDeliveryConditionForDistance(
                Calculations.DistanceBetweenGpsCoordinates(
                    restaurant.GpsLat, restaurant.GpsLong, order.GpsLat, order.GpsLong),
                restaurant.Id
            );
        }

        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {
            var order = new Order();
            order.Id = orderId;
            return await _orderDao.DeleteAsync(order);
        }

        public async Task<(int,Guid)> AddOrderAsync(Order order, IEnumerable<OrderItem> items, Address address, string token)
        {
            // check restaurant id
            var restaurant = await _restaurantDao.FindByIdAsync(order.RestaurantId);
            if (restaurant is null)
                return ((int)AddOrderFailureCode.RestaurantNotFound, Guid.Empty);
            
            // check restaurant menu id's
            var restaurantMenus = await _menuDao.FindAllByRestaurantIdAsync(restaurant.Id);
            var itemsOk = items.All(o => restaurantMenus.Any(m => m.Id == o.MenuId));
            if (!itemsOk)
                return ((int)AddOrderFailureCode.OneOrMoreItemsNotFound, Guid.Empty);

            // check opened status
            var restaurantHasOpen = await IsRestaurantOpen(restaurant.Id);
            if (!restaurantHasOpen)
                return ((int)AddOrderFailureCode.RestaurantHasNotOpened, Guid.Empty);

            // find delivery condition
            var condition = await GetDeliveryConditionForDistance(
                Calculations.DistanceBetweenGpsCoordinates(
                    restaurant.GpsLat, restaurant.GpsLong, order.GpsLat, order.GpsLong),
                restaurant.Id
                );
            if (condition is null) 
                return ((int)AddOrderFailureCode.MatchingDeliveryConditionNotFound, Guid.Empty);

            // check delivery condition
            decimal totalCost = Calculations.CalculateTotalCosts(restaurantMenus, items);
            if (totalCost < condition.MinOrderValue)
                return ((int)AddOrderFailureCode.TotalCostsBelowMinOrderValue, Guid.Empty);

            // create address
            var addressId = await _addressDao.InsertAsync(address);
            address.Id = addressId;
            if (addressId == 0)
                return ((int)AddOrderFailureCode.Default, Guid.Empty);

            // create order
            order.Status = DeliveryStatus.OrderPlaced;
            order.AddressId = addressId;

            // for Requirement No 11, add token, just in free text property
            order.FreeText = token;

            var orderId = await _orderDao.InsertForGuidAsync(order);
            if (orderId == Guid.Empty)
            {
                await _addressDao.DeleteAsync(address);
                return ((int)AddOrderFailureCode.Default, Guid.Empty);
            }

            // create order items
            foreach (var orderItem in items)
            {
                orderItem.OrderId = orderId;
                await _orderItemDao.InsertAsync(orderItem);
            }

            return (1, orderId);
        }

        private async Task<DeliveryCondition?> GetDeliveryConditionForDistance(double deliveryDistance, int restaurantId)
        {
            var deliveryConditions = await _deliveryConditionDao.FindAllByRestaurantIdAsync(restaurantId);
            var sortedDeliveryConditions = deliveryConditions.OrderBy(dc => dc.Distance);
            var condition = sortedDeliveryConditions.FirstOrDefault(dc => dc.Distance >= deliveryDistance);

            return condition;
        }
        private async Task<bool> IsRestaurantOpen(int restaurantId)
        {
            var hours = await _hourDao.FindAllByRestaurantIdAsync(restaurantId);
            var todayHours = hours.Where(h => (int)DateTime.Now.DayOfWeek == (int)h.Day);

            if (!todayHours.Any()) 
                return false;
            else
                return todayHours.Any(h => TimeInRange(DateTime.Now, h));
        }

        private bool TimeInRange(DateTime time, OpeningHour hours)
        {
            return hours.OpenTime < time.TimeOfDay && hours.CloseTime > time.TimeOfDay;
        }

        public enum AddOrderFailureCode
        {
            Default = 0,
            RestaurantNotFound = -1,
            OneOrMoreItemsNotFound = -2,
            MatchingDeliveryConditionNotFound = -3,
            TotalCostsBelowMinOrderValue = -4,
            RestaurantHasNotOpened = -5
        }
    }
}
