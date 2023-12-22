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
    public class AdoOrderDao : IOrderDao
    {
        private AdoTemplate template;

        public AdoOrderDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }

        private Order MapRowToEntity(IDataRecord row)
        {
            object orderedByValue = row["ordered_by"];
            int orderedBy = DBNull.Value.Equals(orderedByValue) ? 0 : (int)orderedByValue;

            return new Order(
                id: (Guid)row["id"],
                restaurantId: (int)row["restaurant_id"],
                addressId: (int)row["address_id"],
                orderedBy: orderedBy,
                timestamp: (DateTime)row["timestamp"],
                gpsLat: (double)row["gps_lat"],
                gpsLong: (double)row["gps_long"],
                freeText: (string)row["free_text"],
                status: (DeliveryStatus)(short)row["status"]);
        }

        /// <summary>
        /// finds all orders by restaurant id
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns>collection of orders if id exists</returns>
        public async Task<IEnumerable<Order>> FindAllByRestaurantIdAsync(int restaurantId)
        {
            string query = "select * from snack_order where restaurant_id=@restaurant_id";

            return await template.QueryAsync(query,
                MapRowToEntity,
                new QueryParameter("@restaurant_id", restaurantId));
        }


        ///// <summary>
        ///// finds all orders by customer id
        ///// </summary>
        ///// <param name="customerId"></param>
        ///// <returns>collection of orders if id exists</returns>
        public async Task<IEnumerable<Order>> FindAllByCustomerIdAsync(int customerId)
        {
            string query = "select * from snack_order where ordered_by=@ordered_by";

            return await template.QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@ordered_by", customerId));
        }
        

        /// <summary>
        /// finds the order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>order if id exists, else null</returns>
        public async Task<Order?> FindByIdAsync(Guid id)
        {
            string query = "select * from snack_order where id = @id";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@id", id));

            return enumerable.SingleOrDefault();
        }


        public async Task<int> InsertAsync(IEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// inserts the given order
        /// </summary>
        /// <param name="entity">Order type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<Guid> InsertForGuidAsync(IEntity entity)
        {
            if (entity is not Order)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Order? order = (Order?)entity;

            string query =
                "insert into snack_order (restaurant_id, address_id, ordered_by, timestamp, gps_lat, gps_long, free_text, status)" +
                "values (@restaurant_id, @address_id, @ordered_by, @timestamp, @gps_lat, @gps_long, @free_text, @status) returning id;";

            //string query =
            //    "insert into snack_order (restaurant_id, address_id, ordered_by, timestamp, gps_lat, gps_long, free_text, status)" +
            //    "values (@restaurant_id, @address_id, @ordered_by, @timestamp, @gps_lat, @gps_long, @free_text, @status); select lastval();";

            return await template.ExecuteScalarForGuidAsync(query,
                new QueryParameter("@restaurant_id", order?.RestaurantId),
                new QueryParameter("@address_id", order?.AddressId),
                new QueryParameter("@ordered_by", order?.OrderedBy),
                new QueryParameter("@timestamp", order?.Timestamp),
                new QueryParameter("@gps_lat", order?.GpsLat),
                new QueryParameter("@gps_long", order?.GpsLong),
                new QueryParameter("@free_text", order?.FreeText),
                new QueryParameter("@status", (short)(order?.Status)));
        }


        /// <summary>
        /// updates the order by id
        /// </summary>
        /// <param name="entity">Order type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<bool> UpdateAsync(IEntity entity)
        {
            if (entity is not Order)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Order? order = (Order?)entity;

            string query =
                "update snack_order set " +
                "id=@id, " +
                "restaurant_id=@restaurant_id, " +
                "address_id=@address_id, " +
                "ordered_by=@ordered_by, " +
                "timestamp=@timestamp, " +
                "gps_lat=@gps_lat, " +
                "gps_long=@gps_long, " +
                "free_text=@free_text, " +
                "status=@status " +
                "where id=@id";

            return await template.ExecuteAsync(query,
                new QueryParameter("@id", order?.Id),
                new QueryParameter("@restaurant_id", order?.RestaurantId),
                new QueryParameter("@address_id", order?.AddressId),
                new QueryParameter("@ordered_by", order?.OrderedBy),
                new QueryParameter("@timestamp", order?.Timestamp),
                new QueryParameter("@gps_lat", order?.GpsLat),
                new QueryParameter("@gps_long", order?.GpsLong),
                new QueryParameter("@free_text", order?.FreeText),
                new QueryParameter("@status", (short)(order?.Status))) == 1;
        }

        /// <summary>
        /// deletes the order by id
        /// </summary>
        /// <param name="entity">requires only the id</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException">in case of object is not a Order</exception>
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            if (entity is not Order)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Order? order = (Order?)entity;

            string query = "delete from snack_order where id=@id";

            return await template.ExecuteAsync(query, new QueryParameter("@id", order?.Id)) == 1;
        }
    }
}
