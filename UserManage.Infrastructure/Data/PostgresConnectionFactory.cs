using Npgsql;
using System.Data;
using UserManage.Infrastructure.Interface;

namespace UserManage.Infrastructure.Data
{

    public class PostgresConnectionFactory(string connectionString) : IDbConnectionFactory
    {
        private readonly string _connectionString = connectionString;

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
