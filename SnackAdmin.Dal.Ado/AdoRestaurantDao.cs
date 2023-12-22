using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Microsoft.Data.SqlClient;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Ado
{

    public class AdoRestaurantDao : IRestaurantDao
    {
        private AdoTemplate template;

        public AdoRestaurantDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }

        private Restaurant MapRowToEntity(IDataRecord row)
        {
            object titleImageValue = row["title_image"];
            var titleImage = DBNull.Value.Equals(titleImageValue) ? new byte[]{} : (byte[])titleImageValue;

            return new Restaurant(
                id: (int) row["id"],
                name: (string) row["name"],
                addressId: (int) row["address_id"],
                gpsLat: (double) row["gps_lat"],
                gpsLong: (double) row["gps_long"],
                webHookUrl: (string) row["webhook_url"],
                titleImage: titleImage,
                apiKey: (string) row["api_key"]);
        }
        
        /// <summary>
        /// finds all restaurants by name
        /// </summary>
        /// <param name="id"></param>
        /// <returns>collection of restaurants</returns>
        public async Task<IEnumerable<Restaurant>> FindAllAsync()
        {
            string query = "select * from restaurant";

            return await template.QueryAsync(query, MapRowToEntity);
        }

        /// <summary>
        /// finds the restaurant by api key
        /// </summary>
        /// <param name="id"></param>
        /// <returns>restaurant if id exists, else null</returns>
        public async Task<Restaurant?> FindByApiKeyAsync(string apiKey)
        {
            string query = "select * from restaurant where api_key = @apiKey";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@apiKey", apiKey));
            return enumerable.SingleOrDefault();
        }

        /// <summary>
        /// finds the restaurant by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>restaurant if id exists, else null</returns>
        public async Task<Restaurant?> FindByIdAsync(int id)
        {
            string query = "select * from restaurant where id=@id";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@id", id));
            return enumerable.SingleOrDefault();
        }

        /// <summary>
        /// finds the restaurant by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>restaurant if name exists, else null</returns>
        public async Task<Restaurant?> FindByNameAsync(string name)
        {
            string query = "select * from restaurant where name = @name";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@name", name));
            return enumerable.FirstOrDefault();
        }

        /// <summary>
        /// inserts the given restaurant
        /// </summary>
        /// <param name="entity">Restaurant type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<int> InsertAsync(IEntity entity)
        {
            if (entity is not Restaurant) 
                throw new NotSupportedException("Entity type not supported for insertion.");

            Restaurant restaurant = (Restaurant)entity;
            //if (restaurant.ApiKey is null)
            //    throw new ArgumentNullException(nameof(restaurant.ApiKey),"Parameter must not be null.");
            //if (restaurant.Name is null)
            //    throw new ArgumentNullException(nameof(restaurant.Name), "Parameter must not be null.");

            string query =
                "insert into restaurant (name, address_id, gps_lat, gps_long, webhook_url, title_image, api_key)" +
                "values (@name, @address_id, @gps_lat, @gps_long, @webhook_url, @title_image, @api_key); select lastval();";

            return await template.ExecuteScalarAsync(query,
                new QueryParameter("@name", restaurant.Name),
                new QueryParameter("@address_id", restaurant.AddressId),
                new QueryParameter("@gps_lat", restaurant.GpsLat),
                new QueryParameter("@gps_long", restaurant.GpsLong),
                new QueryParameter("@webhook_url", restaurant.WebHookUrl),
                new QueryParameter("@title_image", restaurant.TitleImage),
                new QueryParameter("@api_key", restaurant.ApiKey));
        }

        /// <summary>
        /// updates the restaurant by id
        /// </summary>
        /// <param name="entity">Restaurant type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<bool> UpdateAsync(IEntity entity)
        {
            if (entity is not Restaurant) 
                throw new NotSupportedException("Entity type not supported for insertion.");
            Restaurant? restaurant = (Restaurant?)entity;

            string query = 
                "update restaurant set " +
                "name=@name, " +
                "address_id=@address_id, " + 
                "gps_lat=@gps_lat, " +
                "gps_long=@gps_long, " +
                "webhook_url=@webhook_url, " +
                "title_image=@title_image, " +
                "api_key=@api_key " +
                "where id=@id";

            return await template.ExecuteAsync(query,
                new QueryParameter("@id", restaurant?.Id),
                new QueryParameter("@name", restaurant?.Name),
                new QueryParameter("@address_id", restaurant?.AddressId),
                new QueryParameter("@gps_lat", restaurant?.GpsLat),
                new QueryParameter("@gps_long", restaurant?.GpsLong),
                new QueryParameter("@webhook_url", restaurant?.WebHookUrl),
                new QueryParameter("@title_image", restaurant?.TitleImage),
                new QueryParameter("@api_key", restaurant?.ApiKey)) == 1;
        }

        /// <summary>
        /// deletes the restaurant by id
        /// </summary>
        /// <param name="entity">requires only the id</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException">in case of object is not a Restaurant</exception>
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            if (entity is not Restaurant)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Restaurant? restaurant = (Restaurant?)entity;

            string query = "delete from restaurant where id=@id";

            return await template.ExecuteAsync(query, new QueryParameter("@id", restaurant?.Id)) == 1;
        }
    }
}
