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
    public class AdoCustomerDao : ICustomerDao
    {
        private AdoTemplate template;

        public AdoCustomerDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }
        private Customer MapRowToEntity(IDataRecord row) =>
            new Customer(
                id: (int)row["id"],
                userName: (string)row["user_name"],
                passwordHash: (string)row["password_hash"],
                salt: (string)row["salt"]);

        /// <summary>
        /// finds the customer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Customer if id exists, else null</returns>
        public async Task<Customer?> FindByIdAsync(int id)
        {
            string query = "select * from customer where id = @id";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@id", id));

            return enumerable.SingleOrDefault();
        }

        /// <summary>
        /// finds the customer by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<Customer?> FindByUserNameAsync(string userName)
        {
            string query = "select * from customer where user_name = @user_name";

            var enumerable = await template
                .QueryAsync(query,
                    MapRowToEntity,
                    new QueryParameter("@user_name", userName));

            return enumerable.SingleOrDefault();
        }


        /// <summary>
        /// inserts the customer
        /// </summary>
        /// <param name="entity">Customer type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<int> InsertAsync(IEntity entity)
        {
            if (entity is not Customer)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Customer? customer = (Customer?)entity;

            string query =
                "insert into customer (user_name, password_hash, salt)" +
                "values (@user_name, @password_hash, @salt); select lastval();";

            return await template.ExecuteScalarAsync(query,
                new QueryParameter("@user_name", customer?.UserName),
                new QueryParameter("@password_hash", customer?.PasswordHash),
                new QueryParameter("@salt", customer?.Salt));
        }

        /// <summary>
        /// updates the customer by id
        /// </summary>
        /// <param name="entity">Customer type expected</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<bool> UpdateAsync(IEntity entity)
        {
            if (entity is not Customer)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Customer? customer = (Customer?)entity;

            string query =
                "update customer set " +
                "user_name=@user_name, " +
                "password_hash=@password_hash, " +
                "salt=@salt " +
                "where id=@id";

            return await template.ExecuteAsync(query,
                new QueryParameter("@id", customer?.Id),
                new QueryParameter("@user_name", customer?.UserName),
                new QueryParameter("@password_hash", customer?.PasswordHash),
                new QueryParameter("@salt", customer?.Salt)) == 1;
        }

        /// <summary>
        /// deletes the customer by id
        /// </summary>
        /// <param name="entity">requires only the id</param>
        /// <returns>true in case of success</returns>
        /// <exception cref="NotSupportedException">in case of object is not a Customer</exception>
        public async Task<bool> DeleteAsync(IEntity entity)
        {
            if (entity is not Customer)
                throw new NotSupportedException("Entity type not supported for insertion.");
            Customer? customer = (Customer?)entity;

            string query = "delete from customer where id=@id";

            return await template.ExecuteAsync(query, new QueryParameter("@id", customer?.Id)) == 1;
        }
    }
}
