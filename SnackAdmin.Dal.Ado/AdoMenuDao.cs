using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Microsoft.Data.SqlClient;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;

namespace SnackAdmin.Dal.Ado
{
    public class AdoMenuDao : IMenuDao
    {
        private AdoTemplate template;

        public AdoMenuDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }
        private Menu MapRowToEntity(IDataRecord row) => 
            new Menu(
                id: (int)row["id"],
                restaurantId: (int)row["restaurant_id"],
                category: (string)row["category"],
                itemName: (string)row["item_name"],
                description: (string)row["description"],
                price: (decimal)row["price"]);

        /// <summary>
        /// finds all menus by restaurant id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>collection of menus if id exists</returns>
        public async Task<IEnumerable<Menu>> FindAllByRestaurantIdAsync(int id)
        {
            string query = "select * from menu where restaurant_id=@restaurant_id";

            return await template.QueryAsync(query,
                MapRowToEntity,
                new QueryParameter("@restaurant_id", id));
        }

        /// <summary>
        /// finds the menu by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Menu if id exists, else null</returns>
        public async Task<Menu?> FindByIdAsync(int id)
        {
            string query = "select * from menu where id = @id";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@id", id));

            return enumerable.SingleOrDefault();
        }

        /// <summary>
        /// inserts the menu
        /// </summary>
        /// <param name="entity">Menu type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<int> InsertAsync(IEntity entity)
        {
            if (entity is not Menu)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Menu? menu = (Menu?)entity;

            string query =
                "insert into menu (restaurant_id, category, item_name, description, price)" +
                "values (@restaurant_id, @category, @item_name, @description, @price); select lastval();";

            return await template.ExecuteScalarAsync(query,
                new QueryParameter("@restaurant_id", menu?.RestaurantId),
                new QueryParameter("@category", menu?.Category),
                new QueryParameter("@item_name", menu?.ItemName),
                new QueryParameter("@description", menu?.Description),
                new QueryParameter("@price", menu?.Price));
        }

        /// <summary>
        /// updates the menu by id
        /// </summary>
        /// <param name="entity">Menu type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<bool> UpdateAsync(IEntity entity)
        {
            if (entity is not Menu)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Menu? menu = (Menu?)entity;

            string query =
                "update menu set " +
                "restaurant_id=@restaurant_id, " +
                "category=@category, " +
                "item_name=@item_name, " +
                "description=@description, " +
                "price=@price " +
                "where id=@id";

            return await template.ExecuteAsync(query,
                new QueryParameter("@id", menu?.Id),
                new QueryParameter("@restaurant_id", menu?.RestaurantId),
                new QueryParameter("@category", menu?.Category),
                new QueryParameter("@item_name", menu?.ItemName),
                new QueryParameter("@description", menu?.Description),
                new QueryParameter("@price", menu?.Price)) == 1;
        }

        /// <summary>
        /// deletes the menu by id
        /// </summary>
        /// <param name="entity">requires only the id</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException">in case of object is not a Menu</exception>
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            if (entity is not Menu)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Menu? menu = (Menu?)entity;

            string query = "delete from menu where id=@id";

            return await template.ExecuteAsync(query, new QueryParameter("@id", menu?.Id)) == 1;
        }
    }
}
