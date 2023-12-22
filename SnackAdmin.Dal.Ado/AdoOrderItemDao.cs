using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Microsoft.Data.SqlClient;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Ado
{
    public class AdoOrderItemDao : IOrderItemDao
    {
        private AdoTemplate template;

        public AdoOrderItemDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }

        private OrderItem MapRowToEntity(IDataRecord row) =>
            new OrderItem(
                orderId: (Guid)row["order_id"],
                menuId: (int)row["menu_id"],
                quantity: (int)row["quantity"]);

        /// <summary>
        /// finds all order items by order id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>collection of orders items if order id exists</returns>
        public async Task<IEnumerable<OrderItem>> FindAllByOrderIdAsync(Guid orderId)
        {
            return await template.QueryAsync(
                "select * from order_item where order_id=@oid", 
                MapRowToEntity,
                new QueryParameter("@oid", orderId));
        }

        /// <summary>
        /// inserts the order item
        /// </summary>
        /// <param name="entity">OrderItem type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<int> InsertAsync(IEntity entity)
        {
            if (entity is not OrderItem)
                throw new NotSupportedException("Entity type not supported for insertion.");
            OrderItem? orderItem = (OrderItem?)entity;

            string query =
                "insert into order_item (order_id, menu_id, quantity)" +
                "values (@oid, @mid, @qu);";

            return await template.ExecuteAsync(query,
                new QueryParameter("@oid", orderItem?.OrderId),
                new QueryParameter("@mid", orderItem?.MenuId),
                new QueryParameter("@qu", orderItem?.Quantity));
        }

        /// <summary>
        /// updates the order item by order id and menu id
        /// </summary>
        /// <param name="entity">OrderItem type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<bool> UpdateAsync(IEntity entity)
        {
            if (entity is not OrderItem)
                throw new NotSupportedException("Entity type not supported for insertion.");
            OrderItem? oderItem = (OrderItem?)entity;

            string query =
                "update order_item set " +
                "order_id=@oid, " +
                "menu_id=@mid, " +
                "quantity=@qu " +
                "where order_id=@oid and menu_id=@mid";

            return await template.ExecuteAsync(query,
                new QueryParameter("@oid", oderItem?.OrderId),
                new QueryParameter("@mid", oderItem?.MenuId),
                new QueryParameter("@qu", oderItem?.Quantity)) == 1;
        }

        /// <summary>
        /// deletes the order item by order id and menu id
        /// </summary>
        /// <param name="entity">requires order id and menu id</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException">in case of object is not a OrderItem</exception>
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            if (entity is not OrderItem)
                throw new NotSupportedException("Entity type not supported for insertion.");
            OrderItem? orderItem = (OrderItem?)entity;

            string query = "delete from order_item where order_id=@oid and menu_id=@mid";

            return await template.ExecuteAsync(query, 
                new QueryParameter("@oid", orderItem?.OrderId),
                new QueryParameter("@mid", orderItem?.MenuId)) == 1;
        }
    }
}
