using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IBusinessMenuManagementLogic
    {
        Task<Menu?> GetMenuByIdAsync(int menuId);
        Task<int> AddMenuAsync(Menu menu);
        Task<int> UpdateMenuAsync(Menu menu);
        Task<int> DeleteMenuAsync(Menu menu);
    }
}
