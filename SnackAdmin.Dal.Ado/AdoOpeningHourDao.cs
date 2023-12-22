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
    public class AdoOpeningHourDao : IOpeningHourDao
    {
        private AdoTemplate template;

        public AdoOpeningHourDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }
        private OpeningHour MapRowToEntity(IDataRecord row) =>
            new OpeningHour(
                id: (int)row["id"],
                restaurantId: (int)row["restaurant_id"],
                day: (Day)(short)row["day"],
                openTime: (TimeSpan)row["open_time"],
                closeTime: (TimeSpan)row["close_Time"]);

        /// <summary>
        /// finds all OpeningHour by restaurant id
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns>collection of OpeningHour if id exists</returns>
        public async Task<IEnumerable<OpeningHour>> FindAllByRestaurantIdAsync(int restaurantId)
        {
            string query = "select * from opening_hours where restaurant_id=@restaurant_id";

            return await template.QueryAsync(query,
                MapRowToEntity,
                new QueryParameter("@restaurant_id", restaurantId));
        }

        /// <summary>
        /// finds the OpeningHour by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>OpeningHour if id exists, else null</returns>
        public async Task<OpeningHour?> FindByIdAsync(int id)
        {
            string query = "select * from opening_hours where id = @id";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@id", id));

            return enumerable.SingleOrDefault();
        }

        /// <summary>
        /// inserts the given OpeningHour
        /// </summary>
        /// <param name="entity">OpeningHour type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<int> InsertAsync(IEntity entity)
        {
            if (entity is not OpeningHour)
                throw new NotSupportedException("Entity type not supported for insertion.");
            OpeningHour openingHour = (OpeningHour)entity;

            string query =
                "insert into opening_hours (restaurant_id, day, open_time, close_Time)" +
                "values (@restaurant_id, @day, @open_time, @close_Time); select lastval();";

            return await template.ExecuteScalarAsync(query,
                new QueryParameter("@restaurant_id", openingHour.RestaurantId),
                new QueryParameter("@day", (short)openingHour.Day),
                new QueryParameter("@open_time", openingHour.OpenTime),
                new QueryParameter("@close_Time", openingHour.CloseTime));
        }

        /// <summary>
        /// updates the OpeningHour by id
        /// </summary>
        /// <param name="entity">OpeningHour type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<bool> UpdateAsync(IEntity entity)
        {
            if (entity is not OpeningHour)
                throw new NotSupportedException("Entity type not supported for insertion.");
            OpeningHour openingHour = (OpeningHour)entity;

            string query =
                "update opening_hours set " +
                "restaurant_id=@restaurant_id, " +
                "day=@day, " +
                "open_time=@open_time, " +
                "close_Time=@close_Time " +
                "where id=@id";

            return await template.ExecuteAsync(query,
                new QueryParameter("@id", openingHour.Id),
                new QueryParameter("@restaurant_id", openingHour.RestaurantId),
                new QueryParameter("@day", (short)openingHour.Day),
                new QueryParameter("@open_time", openingHour.OpenTime),
                new QueryParameter("@close_Time", openingHour.CloseTime)) == 1;
        }

        /// <summary>
        /// deletes the OpeningHour by id
        /// </summary>
        /// <param name="entity">requires only the id</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException">in case of object is not a OpeningHour</exception>
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            if (entity is not OpeningHour)
                throw new NotSupportedException("Entity type not supported for insertion.");
            OpeningHour openingHour = (OpeningHour)entity;

            string query = "delete from opening_hours where id=@id";

            return await template.ExecuteAsync(query, new QueryParameter("@id", openingHour.Id)) == 1;
        }
    }
}
