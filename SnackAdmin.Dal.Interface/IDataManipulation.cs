using SnackAdmin.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Dal.Interface
{
    public interface IDataManipulation
    {
        Task<int> InsertAsync(IEntity entity);
        Task<bool> UpdateAsync(IEntity entity);
        Task<bool> DeleteAsync(IEntity entity); // int id is not enough e.g. order_item PK = orderId + menuId
    }
}
