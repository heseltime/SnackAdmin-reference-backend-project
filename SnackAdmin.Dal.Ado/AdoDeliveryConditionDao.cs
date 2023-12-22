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
    public class AdoDeliveryConditionDao : IDeliveryCondition
    {
        private AdoTemplate template;

        public AdoDeliveryConditionDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }
        private DeliveryCondition MapRowToEntity(IDataRecord row) =>
            new DeliveryCondition(
                id: (int)row["id"],
                restaurantId: (int)row["restaurant_id"],
                distance: (double)row["distance"],
                minOrderValue: (decimal)row["min_order_value"],
                deliveryCost: (decimal)row["delivery_cost"]);

        /// <summary>
        /// finds all delivery_condition by restaurant id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>collection of DeliveryCondition if id exists</returns>
        public async Task<IEnumerable<DeliveryCondition>> FindAllByRestaurantIdAsync(int restaurantId)
        {
            string query = "select * from delivery_condition where restaurant_id=@rid";

            return await template.QueryAsync(query,
                MapRowToEntity,
                new QueryParameter("@rid", restaurantId));
        }


        /// <summary>
        /// finds the delivery condition by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DeliveryCondition if id exists, else null</returns>
        public async Task<DeliveryCondition?> FindByIdAsync(int id)
        {
            string query = "select * from delivery_condition where id = @id";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@id", id));

            return enumerable.SingleOrDefault();
        }


        /// <summary>
        /// inserts the given delivery condition
        /// </summary>
        /// <param name="entity">DeliveryCondition type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<int> InsertAsync(IEntity entity)
        {
            if (entity is not DeliveryCondition)
                throw new NotSupportedException("Entity type not supported for insertion.");
            DeliveryCondition? condition = (DeliveryCondition?)entity;

            string query =
                "insert into delivery_condition (restaurant_id, distance, min_order_value, delivery_cost)" +
                "values (@rid, @dist, @min, @cost); select lastval();";

            return await template.ExecuteScalarAsync(query,
                new QueryParameter("@id", condition?.Id),
                new QueryParameter("@rid", condition?.RestaurantId),
                new QueryParameter("@dist", condition?.Distance),
                new QueryParameter("@min", condition?.MinOrderValue),
                new QueryParameter("@cost", condition?.DeliveryCost));
        }

        /// <summary>
        /// updates the DeliveryCondition by id
        /// </summary>
        /// <param name="entity">DeliveryCondition type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<bool> UpdateAsync(IEntity entity)
        {
            if (entity is not DeliveryCondition)
                throw new NotSupportedException("Entity type not supported for insertion.");
            DeliveryCondition? condition = (DeliveryCondition?)entity;

            string query =
                "update delivery_condition set " +
                //"restaurant_id=@rid, " +
                "distance=@dist, " +
                "min_order_value=@min, " +
                "delivery_cost=@cost " +
                "where id=@id";

            return await template.ExecuteAsync(query,
                new QueryParameter("@id", condition?.Id),
                //new QueryParameter("@rid", condition?.RestaurantId),
                new QueryParameter("@dist", condition?.Distance),
                new QueryParameter("@min", condition?.MinOrderValue),
                new QueryParameter("@cost", condition?.DeliveryCost)) == 1;
        }

        /// <summary>
        /// deletes the delivery condition by id
        /// </summary>
        /// <param name="entity">requires only the id</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException">in case of object is not a DeliveryCondition</exception>
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            if (entity is not DeliveryCondition)
                throw new NotSupportedException("Entity type not supported for insertion.");
            DeliveryCondition? condition = (DeliveryCondition?)entity;

            string query = "delete from delivery_condition where id=@id";

            return await template.ExecuteAsync(query, new QueryParameter("@id", condition?.Id)) == 1;
        }
    }
}
