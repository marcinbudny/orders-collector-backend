using System;
using System.IO;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersCollector.Publishing;
using Serilog;

namespace OrdersCollector.Api
{
    public static class ApiHost
    {
        public static void Run(IConfiguration config) => CreateWebHost(config).Run();

        public static IWebHost CreateWebHost(IConfiguration config, Action<IServiceCollection> customizeServices = null)
        {
            return new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseUrls("http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSerilog()
                .ConfigureServices(services =>
                {
                    services.AddCors();
                    services.AddConfigration(config);
                    services.AddOrdersCollector();
                    services.AddSignalR();

                    services.AddCarter();
                    
                    customizeServices?.Invoke(services);
                })
                .Configure(app =>
                {
                    app.UseCors(policy => policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                    app.UseSignalR(cfg =>
                        cfg.MapHub<EventsHub>("/events"));
                    app.UseCarter();
                })
                .Build();
        }
    }
}