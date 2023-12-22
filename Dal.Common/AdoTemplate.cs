using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using SnackAdmin.Dal.Interface;

namespace Dal.Common
{
    public delegate T RowMapper<T>(IDataRecord row);

    public class AdoTemplate
    {

        private readonly IConnectionFactory connectionFactory;
        public AdoTemplate(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        private void AddParameters(DbCommand command, QueryParameter[] parameters)
        {
            foreach (var p in parameters)
            {
                DbParameter dbParameter = command.CreateParameter();
                dbParameter.ParameterName = p.Name;
                dbParameter.Value = p.Value;
                command.Parameters.Add(dbParameter);
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, RowMapper<T> rowMapper, params QueryParameter[] parameters)
        {
            await using DbConnection dbConnection = await connectionFactory.CreateConnectionAsync();

            await using DbCommand command = dbConnection.CreateCommand();
            command.CommandText = sql;
            AddParameters(command, parameters);

            await using DbDataReader reader = command.ExecuteReader();

            var items = new List<T>();

            while (await reader.ReadAsync())
            {
                items.Add(rowMapper(reader));
            }

            return items;
        }

        public async Task<T?> QuerySingleAsync<T>(string sql, RowMapper<T> rowMapper, params QueryParameter[] parameters)
        {
            var enumerable = await QueryAsync(sql, rowMapper, parameters);
            return enumerable.SingleOrDefault();
        }

        public async Task<int> ExecuteAsync(string sql, params QueryParameter[] parameters)
        {
            await using DbConnection dbConnection = await connectionFactory.CreateConnectionAsync();

            await using DbCommand command = dbConnection.CreateCommand();
            command.CommandText = sql;
            AddParameters(command, parameters);

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> ExecuteScalarAsync(string sql, params QueryParameter[] parameters)
        {
            await using DbConnection conn = await connectionFactory.CreateConnectionAsync();

            await using DbCommand command = conn.CreateCommand();
            command.CommandText = sql;
            AddParameters(command, parameters);

            var obj = await command.ExecuteScalarAsync();

            return Convert.ToInt32(obj);
        }
        public async Task<Guid> ExecuteScalarForGuidAsync(string sql, params QueryParameter[] parameters)
        {
            await using DbConnection conn = await connectionFactory.CreateConnectionAsync();

            await using DbCommand command = conn.CreateCommand();
            command.CommandText = sql;
            AddParameters(command, parameters);

            var obj = await command.ExecuteScalarAsync();
            bool isValidGuid = Guid.TryParse(obj?.ToString(), out var parsedGuid);

            if (isValidGuid)
                return parsedGuid;

            return Guid.Empty;
        }
    }
}
