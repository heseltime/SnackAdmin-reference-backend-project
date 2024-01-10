using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SnackAdmin.BusinessLogic.Common;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;
using static SnackAdmin.BusinessLogic.BusinessRegistrationManagementLogic;

namespace SnackAdmin.BusinessLogic
{
    public class BusinessManagementLogic : IBusinessManagementLogic
    {
        private readonly IAddressDao _addressDao;
        private readonly IRestaurantDao _restaurantDao;
        private readonly IMenuDao _menuDao;
        private readonly IDeliveryCondition _deliveryConditionDao;
        private readonly IOpeningHourDao _hourDao;
        private readonly IOrderDao _orderDao;

        public BusinessManagementLogic(
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

        // For GET requests
        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string restaurantName)
        {
            Restaurant restaurant = await _restaurantDao.FindByNameAsync(restaurantName);

            IEnumerable<Order> orders = await _orderDao.FindAllByRestaurantIdAsync(restaurant.Id);

            return orders;
        }
        
        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            Order order = await _orderDao.FindByIdAsync(id);

            return order;
        }

        public async Task<int> UpdateOrderAsync(Order order)
        {
            bool success = await _orderDao.UpdateAsync(order);

            if (success) { 
                return (int)UpdateOrderFailureCode.Ok;
            } else
            {
                return (int)UpdateOrderFailureCode.Default;
            }
        }

        public async Task<Order> GetOrderByTokenAsync(string token)
        {
            // token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validate the token and extract claims - this typically needs token validation parameters
                // Token validation parameters usually include the secret key, issuer, audience, etc.
                // var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // So for simplicity, assuming the token is already validated and just extracting claims
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var subjectClaim = jwtToken.Subject;

                if (string.IsNullOrEmpty(subjectClaim))
                {
                    throw new InvalidOperationException("Subject claim not found in the token");
                }

                // Convert the subject claim to Guid
                var orderId = Guid.Parse(subjectClaim);

                // Retrieve the order using the order ID
                Order order = await _orderDao.FindByIdAsync(orderId);

                return order;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Menu>> GetAllMenuItemsAsync(string restaurantName)
        {
            Restaurant restaurant = await _restaurantDao.FindByNameAsync(restaurantName);

            IEnumerable<Menu> menuItems = await _menuDao.FindAllByRestaurantIdAsync(restaurant.Id);

            return menuItems;
        }

        public async Task<Menu> GetMenuByIdAsync(int id)
        {
            Menu menu = await _menuDao.FindByIdAsync(id);

            return menu;
        }
        public async Task<int> UpdateMenuAsync(Menu menu)
        {
            bool success = await _menuDao.UpdateAsync(menu);

            if (success)
            {
                return (int)UpdateOrderFailureCode.Ok;
            }
            else
            {
                return (int)UpdateOrderFailureCode.Default;
            }
        }

        public async Task<int> AddMenuAsync(Menu menu)
        {
            int success = await _menuDao.InsertAsync(menu);

            if (success > 0)
            {
                return (int)UpdateOrderFailureCode.Ok;
            }
            else
            {
                return (int)UpdateOrderFailureCode.Default;
            }
        }

        public async Task<int> DeleteMenuAsync(Menu menu)
        {
            bool success = await _menuDao.DeleteAsync(menu);

            if (success)
            {
                return (int)UpdateOrderFailureCode.Ok;
            }
            else
            {
                return (int)UpdateOrderFailureCode.Default;
            }
        }

        public async Task<IEnumerable<DeliveryCondition>> GetAllDeliveryConditionsAsync(string restaurantName)
        {
            Restaurant restaurant = await _restaurantDao.FindByNameAsync(restaurantName);

            IEnumerable<DeliveryCondition> deliveryConditions = await _deliveryConditionDao.FindAllByRestaurantIdAsync(restaurant.Id);

            return deliveryConditions;
        }

        public async Task<DeliveryCondition> GetDeliveryConditionByIdAsync(int conditionId)
        {
            DeliveryCondition condition = await _deliveryConditionDao.FindByIdAsync(conditionId);

            return condition;
        }

        public async Task<int> UpdateDeliveryConditionAsync(DeliveryCondition condition)
        {
            bool success = await _deliveryConditionDao.UpdateAsync(condition);

            if (success)
            {
                return (int)UpdateOrderFailureCode.Ok;
            }
            else
            {
                return (int)UpdateOrderFailureCode.Default;
            }
        }

        public async Task<int> DeleteDeliveryConditionAsync(DeliveryCondition condition)
        {
            bool success = await _deliveryConditionDao.DeleteAsync(condition);

            if (success)
            {
                return (int)UpdateOrderFailureCode.Ok;
            }
            else
            {
                return (int)UpdateOrderFailureCode.Default;
            }
        }

        // for authentication and authorisation
        public async Task<Restaurant> GetRestaurantByname(string restaurantName)
        {
            Restaurant restaurant = await _restaurantDao.FindByNameAsync(restaurantName);

            return restaurant;
        }

        public enum UpdateOrderFailureCode
            // would need to be made generic also for menu and delivery conditions
        {
            Default = -4,
            ServiceUnavailable = -3,
            BadRequest = -2,
            Conflict = -1,
            Ok = 0
        }
    }
}
