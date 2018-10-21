using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Marten;
using Marten.Events.Projections.Async;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Npgsql;
using OrdersCollector.Api;
using OrdersCollector.Api.Contract;
using OrdersCollector.Reads;
using OrdersCollector.Tests.Utils;
using OrdersCollector.Utils;

namespace OrdersCollector.Tests.Integration
{
    public class Fixture : IDisposable
    {
        private readonly string _connectionString;
        private readonly IWebHost _host;

        protected IDocumentStore Store  => _host?.Services.GetService<IDocumentStore>();
        protected MockTimeProvider TimeProvider { get; }

        protected string BaseAddress { get; } = "http://localhost:5000";

        protected Fixture()
        {
            var config = ConfigLoader.Load("appsettings.tests.json");

            _connectionString = config.GetSection("Database").GetValue<string>("ConnectionString");

            DatabaseCreator.CreateIfNotExists(_connectionString).Wait();
            DatabaseCleanup.ClearDatabase(_connectionString);

            TimeProvider = new MockTimeProvider();

            _host = ApiHost.CreateWebHost(config, services => services.AddSingleton<ITimeProvider>(TimeProvider));
            _host.Start();
        }

        protected async Task PostToSutApi<TRequest>(string path, TRequest request)
        {
            var response = await BaseAddress
                .AppendPathSegment(path)
                .AllowAnyHttpStatus()
                .PostJsonAsync(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<Error>(responseString);
                
                throw new TestApiCalErrorException(error);
            }
            
            response.EnsureSuccessStatusCode();
        }

        protected void It_is_now(DateTime dateTime) => TimeProvider.UtcNow = dateTime;

        protected async Task Wait_for_projection_to_process_events()
            => await _host.Services.GetService<ProjectionHostedService>().WaitForNonStaleResults();
            
        protected async Task Event_stream_should_have_these_events(string streamId, params object[] expected)
        {
            using (var session = Store.OpenSession())
            {
                var events = (await session.Events.FetchStreamAsync(streamId)).Select(e => e.Data).ToList();
                events.Should().BeEquivalentTo(expected, cfg => cfg);
                events.Select(e => e.GetType()).Should().BeEquivalentTo(expected.Select(e => e.GetType()));
            }
        }

        protected async Task Read_model_should_have_this_document<TDocument>(string id, Action<TDocument> verification)
        {
            using(var session = Store.OpenSession()) 
            {
                var doc = await session.LoadAsync<TDocument>(id);
                verification(doc);
            }
        }
        
        protected async Task Read_model_should_not_have_this_document<TDocument>(string id)
        {
            using(var session = Store.OpenSession()) 
            {
                var doc = await session.LoadAsync<TDocument>(id);
                doc.Should().BeNull(because: "document should not exist");
            }
        }


        protected NpgsqlConnection GetOpenConnection(string connectionString = null)
        {
            var conn = new NpgsqlConnection(connectionString ?? _connectionString);
            conn.Open();
            return conn;
        }
        
        public virtual void Dispose() => _host?.Dispose();
    }
}