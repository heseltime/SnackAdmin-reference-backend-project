using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IBusinessRegistrationManagementLogic
    {
        Task<(int, Restaurant)> AddRestaurantAsync(RestaurantForCreation restaurant);
    }
}
