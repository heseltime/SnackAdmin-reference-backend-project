using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class OrderItem : IEntity
    {
        public OrderItem()
        {
            
        }

        public OrderItem(Guid orderId, int menuId, int quantity)
        {
            OrderId = orderId;
            MenuId = menuId;
            Quantity = quantity;
        }

        public Guid OrderId { get; set; }
        public int MenuId { get; set; }
        public int Quantity { get; set; }


        public override string ToString() => 
            $"[OrderItem {OrderId}, {MenuId}, {Quantity}]";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            OrderItem otherOrderItem = (OrderItem)obj;

            return OrderId == otherOrderItem.OrderId &&
                   MenuId == otherOrderItem.MenuId &&
                   Quantity == otherOrderItem.Quantity;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId, MenuId, Quantity);
        }


    }

}
