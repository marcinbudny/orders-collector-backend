using System.IO;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;

namespace OrdersCollector
{
    public static class ConfigLoader 
    {
        public static IConfiguration Load(string additionalJsonFile = null)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            if (additionalJsonFile != null)
                builder.AddJsonFile(additionalJsonFile, optional: true);
                    
            return builder
                .AddEnvironmentVariables()
                .Build();
        }
    }
}