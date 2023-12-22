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
    public class AdoAddressDao : IAddressDao
    {
        private AdoTemplate template;

        public AdoAddressDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }
        private Address MapRowToEntity(IDataRecord row) =>
            new Address(
                id: (int)row["id"],
                street: (string)row["street"],
                postalCode: (string)row["postal_code"],
                city: (string)row["city"],
                state: (string)row["state"],
                country: (string)row["country"]);

        /// <summary>
        /// finds the address by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Address if id exists, else null</returns>
        public async Task<Address?> FindByIdAsync(int id)
        {
            string query = "select * from address where id = @id";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@id", id));

            return enumerable.SingleOrDefault();
        }

        /// <summary>
        /// Helper fn to get id based on a street name, used for testing
        /// </summary>
        /// <param name="street"></param>
        /// <returns>Address if id exists, else null</returns>
        public async Task<Address?> FindByStreetAsync(string street)
        {
            string query = "select * from address where street = @street";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@street", street));

            return enumerable.SingleOrDefault();
        }

        /// <summary>
        /// inserts the address
        /// </summary>
        /// <param name="entity">Address type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<int> InsertAsync(IEntity entity)
        {
            if (entity is not Address)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Address? address = (Address?)entity;

            string query =
                "insert into address (street, postal_code, city, state, country)" +
                "values (@street, @postal_code, @city, @state, @country); select lastval();";

            return await template.ExecuteScalarAsync(query,
                new QueryParameter("@street", address?.Street),
                new QueryParameter("@postal_code", address?.PostalCode),
                new QueryParameter("@city", address?.City),
                new QueryParameter("@state", address?.State),
                new QueryParameter("@country", address?.Country));
        }

        /// <summary>
        /// updates the address by id
        /// </summary>
        /// <param name="entity">Address type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<bool> UpdateAsync(IEntity entity)
        {
            if (entity is not Address)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Address? address = (Address?)entity;

            string query =
                "update address set " +
                "street=@street, " +
                "postal_code=@postal_code, " +
                "city=@city, " +
                "state=@state, " +
                "country=@country " +
                "where id=@id";

            return await template.ExecuteAsync(query,
                new QueryParameter("@id", address?.Id),
                new QueryParameter("@street", address?.Street),
                new QueryParameter("@postal_code", address?.PostalCode),
                new QueryParameter("@city", address?.City),
                new QueryParameter("@state", address?.State),
                new QueryParameter("@country", address?.Country)) == 1;
        }

        /// <summary>
        /// deletes the address by id
        /// </summary>
        /// <param name="entity">requires only the id</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException">in case of object is not a Address</exception>
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            if (entity is not Address)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Address? address = (Address?)entity;

            string query = "delete from address where id=@id";

            return await template.ExecuteAsync(query, new QueryParameter("@id", address?.Id)) == 1;
        }
    }
}
