using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Carter;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrdersCollector.Api;
using OrdersCollector.Config;
using OrdersCollector.Reads;
using OrdersCollector.Utils;
using Serilog;

namespace OrdersCollector
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var config = ConfigLoader.Load();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();
            
                Log.Information("Orders collector starting...");

                ApiHost.Run(config);

            }
            catch (Exception e)
            {
                Log.Fatal(e, "Error on startup");
                Console.WriteLine("Error on startup " + e);
                throw;
            }
            
            Log.Information("Bye bye");
        }
    }

}