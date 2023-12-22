using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Interfaces
{
    public interface IBusinessDeliveryManagementLogic
    {
        Task<DeliveryCondition?> GetDeliveryConditionByIdAsync(int conditionId);
        Task<int> AddDeliveryConditionAsync(DeliveryCondition condition);
        Task<int> UpdateDeliveryConditionAsync(DeliveryCondition condition);
        Task<int> DeleteDeliveryConditionAsync(DeliveryCondition condition);
    }
}
