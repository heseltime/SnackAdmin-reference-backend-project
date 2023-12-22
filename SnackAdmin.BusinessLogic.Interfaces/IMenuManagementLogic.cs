using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IMenuManagementLogic
    {
        Task<IEnumerable<Menu>> GetMenusAsync(int restaurantId);
    }
}
