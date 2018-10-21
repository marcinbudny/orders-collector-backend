using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Marten.Events.Projections.Async;
using Marten.Storage;
using Npgsql;
using OrdersCollector.Domain.Order.Events.V1;
using OrdersCollector.Utils;

namespace OrdersCollector.Reads.Relational
{
    public class RelationalProjection : IProjection
    {
        private readonly string _connectionString;
        private readonly SchemaCreator _schemaCreator;

        public RelationalProjection(string connectionString, SchemaCreator schemaCreator)
        {
            _connectionString = connectionString;
            _schemaCreator = schemaCreator;
        }

        // TODO: improve infrastructure around IProjection
        public Type[] Consumes { get; } =
            {
                typeof(NewOrderStarted), 
                typeof(OrderItemAdded),
                typeof(OrderResponsiblePersonSelected),
                typeof(OrderResponsiblePersonRemoved),
                typeof(OrderItemRemoved),
            };

        public void EnsureStorageExists(ITenant tenant) => _schemaCreator.Create();

        public async Task ApplyAsync(IDocumentSession session, EventPage page, CancellationToken token)
        {
            var commands = EventPageToCommands(page);
            await ExecuteCommands(commands);
        }

        private IEnumerable<NpgsqlCommand> EventPageToCommands(EventPage page)
        {
            return page.Events
                .Select(EventToDbCommand)
                .ToArray();
        }

        private NpgsqlCommand EventToDbCommand(IEvent @event)
        {
            switch (@event.Data)
            {
                case NewOrderStarted data:
                    return ToCommand(data);
                case OrderItemAdded data:
                    return ToCommand(data);
                case OrderResponsiblePersonSelected data:
                    return ToCommand(data);
                case OrderResponsiblePersonRemoved data:
                    return ToCommand(data);
                case OrderItemRemoved data:
                    return ToCommand(data);
                default:
                    throw new InvalidOperationException($"Unknown event {@event.Data.GetType()}");
            }
        }

        private NpgsqlCommand ToCommand(NewOrderStarted data)
        {
            var command = new NpgsqlCommand("INSERT INTO reads.order (id, date, local_id) values (@Id, @Date, @LocalId)");
            command.Parameters.Add(new NpgsqlParameter("@Id", data.OrderId));
            command.Parameters.Add(new NpgsqlParameter("@Date", data.Date.Date));
            command.Parameters.Add(new NpgsqlParameter("@LocalId", data.LocalId));

            return command;
        }

        private NpgsqlCommand ToCommand(OrderItemAdded data)
        {
            var command = new NpgsqlCommand("INSERT INTO reads.order_item (person_name, item_name, added_at, order_id) values (@PersonName, @ItemName, @AddedAt, @OrderId)");
            command.Parameters.Add(new NpgsqlParameter("@PersonName", data.PersonName));
            command.Parameters.Add(new NpgsqlParameter("@ItemName", data.ItemName));
            command.Parameters.Add(new NpgsqlParameter("@AddedAt", data.AddedAt));
            command.Parameters.Add(new NpgsqlParameter("@OrderId", data.OrderId));

            return command;
        }

        private NpgsqlCommand ToCommand(OrderResponsiblePersonSelected data)
        {
            var command = new NpgsqlCommand("UPDATE reads.order set responsible_person = @ResponsiblePerson where id = @Id");
            command.Parameters.Add(new NpgsqlParameter("@ResponsiblePerson", data.PersonName));
            command.Parameters.Add(new NpgsqlParameter("@Id", data.OrderId));

            return command;
        } 

        private NpgsqlCommand ToCommand(OrderResponsiblePersonRemoved data)
        {
            var command = new NpgsqlCommand("UPDATE reads.order set responsible_person = NULL where id = @Id");
            command.Parameters.Add(new NpgsqlParameter("@Id", data.OrderId));

            return command;
        } 

        private NpgsqlCommand ToCommand(OrderItemRemoved data)
        {
            var command = new NpgsqlCommand("DELETE FROM reads.order_item WHERE order_id = @Id and person_name = @PersonName");
            command.Parameters.Add(new NpgsqlParameter("@PersonName", data.PersonName));
            command.Parameters.Add(new NpgsqlParameter("@Id", data.OrderId));

            return command;
        } 

        private async Task ExecuteCommands(IEnumerable<NpgsqlCommand> commands)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var tx = conn.BeginTransaction())
                {
                    await commands.ForEachAsync(async cmd =>
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = tx;

                        await cmd.ExecuteNonQueryAsync();
                    });

                    await tx.CommitAsync();
                }
            }
        }

        public AsyncOptions AsyncOptions { get; } = new AsyncOptions();

        public void Apply(IDocumentSession session, EventPage page) => 
            throw new NotImplementedException("Only ApplyAsync is implemented");

    }
    
}