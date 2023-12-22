using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic
{
    public class MenuManagementLogic : IMenuManagementLogic
    {
        private readonly IMenuDao _manuDao;

        public MenuManagementLogic(IMenuDao manuDao)
        {
            _manuDao = manuDao;
        }

        public async Task<IEnumerable<Menu>> GetMenusAsync(int restaurantId)
        {
            return await _manuDao.FindAllByRestaurantIdAsync(restaurantId);
        }
    }
}
