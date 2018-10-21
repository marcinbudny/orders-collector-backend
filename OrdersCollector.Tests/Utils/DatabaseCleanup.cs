using Npgsql;
using OrdersCollector.Utils;

namespace OrdersCollector.Tests.Utils
{
    public class DatabaseCleanup
    {
        public static void ClearDatabase(string connectionString)
        {
            // this version has problem with:
            // Npgsql.PostgresException : 55006: database "orders_collector_tests" is being accessed by other users
            
//            var builder = new NpgsqlConnectionStringBuilder(connectionString);
//            var testDatabase = builder.Database;
//
//            builder.Database = "postgres";
//            var postgresConnectionString = builder.ConnectionString;
//
//            using (var connection = GetOpenConnection(postgresConnectionString))
//            {
//                var exists = (int)new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{testDatabase}'", connection).ExecuteScalar();
//                if (exists == 1)
//                    new NpgsqlCommand($"DROP DATABASE {testDatabase}", connection).ExecuteNonQuery();
//                new NpgsqlCommand($"CREATE DATABASE {testDatabase}", connection).ExecuteNonQuery();
//            }


            // TODO: autodiscover?
            var tablesToClear = new[]
            {
                (tableName: "mt_event_progression",  schema: "public"), 
                (tableName: "mt_events",             schema: "public"), 
                (tableName: "mt_streams",            schema: "public"), 
                (tableName: "mt_doc_order",          schema: "public"),
                (tableName: "mt_doc_local",          schema: "public"),
                (tableName: "order",                 schema: "reads"),
                (tableName: "order_item",            schema: "reads"),
            };

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                
                var cmdText = "";
                tablesToClear.ForEach(table =>
                {
                    cmdText += $@"DO $$
                                     BEGIN
                                     IF EXISTS (
                                        SELECT * FROM information_schema.tables 
                                        WHERE table_schema = '{table.schema}' and table_name='{table.tableName}') 
                                        
                                        THEN
                                            DELETE FROM {table.schema}.{table.tableName};
                                        END IF;
                                     END
                                     $$;";

                    
                });

                cmdText += @"DO $$
                    BEGIN
                    IF EXISTS (SELECT 1 FROM pg_class where relname = 'mt_events_sequence' )
                    THEN
                      ALTER SEQUENCE mt_events_sequence RESTART WITH 1;
                    END IF;
                    END
                    $$;";
                
                new NpgsqlCommand(cmdText, connection).ExecuteNonQuery();
            }
        }
    }
}