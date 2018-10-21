using Marten;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrdersCollector.Config;
using OrdersCollector.Domain.Local;
using OrdersCollector.Domain.Order;
using OrdersCollector.Publishing;
using OrdersCollector.Reads;
using OrdersCollector.Utils;

namespace OrdersCollector
{
    public static class OrdersCollectorServicesExtensions
    {

        public static IServiceCollection AddConfigration(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config.GetSection("Database").Get<DatabaseOptions>());
            return services;
        }
        
        public static IServiceCollection AddOrdersCollector(this IServiceCollection services)
        {
            services.AddSingleton<ITimeProvider, TimeProvider>();
            
            services.AddSingleton<IDocumentStore>(provider =>
            {
                var connectionString = provider.GetService<DatabaseOptions>().ConnectionString;
                var hubContext = provider.GetService<IHubContext<EventsHub>>();
                return DocumentStoreFactory.Create(connectionString, hubContext);
            });
            services.AddSingleton<ProjectionHostedService>();
            services.AddSingleton<IHostedService>(provider => provider.GetService<ProjectionHostedService>());

            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
            
            services.AddSingleton<OrderHandler>();
            services.AddSingleton<LocalHandler>();

            return services;
        }    
    }
}