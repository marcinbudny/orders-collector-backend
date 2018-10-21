using System.Threading.Tasks;
using Npgsql;

namespace OrdersCollector.Utils
{
    public static class DatabaseCreator
    {
        public static async Task CreateIfNotExists(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var database = builder.Database;

            builder.Database = "postgres";
            var postgresConnectionString = builder.ConnectionString;
            
            using (var connection = new NpgsqlConnection(postgresConnectionString))
            {
                await connection.OpenAsync();
                
                var exists = 
                    await new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{database}'", connection).ExecuteScalarAsync();
                if (exists == null)
                    await new NpgsqlCommand($"CREATE DATABASE {database}", connection).ExecuteNonQueryAsync();
            }

        }
    }
}