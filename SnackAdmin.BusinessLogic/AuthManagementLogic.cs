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
    public class AuthManagementLogic : IAuthManagementLogic
    {
        private readonly IAddressDao _addressDao;
        private readonly IRestaurantDao _restaurantDao;
        private readonly IMenuDao _menuDao;
        private readonly IDeliveryCondition _deliveryConditionDao;
        private readonly IOpeningHourDao _hourDao;
        private readonly IOrderDao _orderDao;

        public AuthManagementLogic(
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

        public async Task<bool> IsValidUser(Login login)
        {
            string restaurantName = login.RestaurantName;

            // Check provided api key against the one stored in db: 
            //      if they are the same, confirm is valid use to issue token
            string providedApiKey = login.ApiKey;

            Restaurant restaurant = await _restaurantDao.FindByNameAsync(restaurantName);

            if (restaurant == null || restaurant.ApiKey != providedApiKey)
            {
                return false;
            } else
            {
                return true;
            }
            
        }
    }
}
