using System;
using System.IO;
using Npgsql;

namespace OrdersCollector.Reads.Relational 
{
    public class SchemaCreator
    {
        private readonly string _connectionString;

        public SchemaCreator(string connectionString) => _connectionString = connectionString;

        public void Create() 
        {
            var ddl = ReadSchemaDdl();
            Execute(ddl);
        }

        private void Execute(string ddl)
        {
            using(var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(ddl, conn);
                cmd.ExecuteNonQuery();
            }
        }

        private string ReadSchemaDdl() => File.ReadAllText("Reads/Relational/schema.sql");
    }
}