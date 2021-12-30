using ETL;
using ETL.Repository;
using ETL.Utils;
using ETL.Utils.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETLTests
{
    public class TestSuit<T>
    {
        public T GetConfiguratedComponent(string profile)
        {
            var servicesProvider = ConfigureServices(profile);
            return servicesProvider.GetService<T>();
        }

        /// <summary>
        /// Utility function to build service provider for dependency injection
        /// </summary>
        /// <returns></returns>
        private  ServiceProvider ConfigureServices(string profile)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting(profile).Build();
            var serviceProvier = new ServiceCollection()
                .Configure<Directories>(config.GetSection("Directories"))
                .Configure<DatabaseSettings>(config.GetSection("DatabaseSettings"))

                .AddSingleton<ILogger>(sp => new Logger("ETL-process"))
                .AddSingleton<FileManager>()
                .AddSingleton<Deserializer>()
                .AddSingleton<Repository>()
                .BuildServiceProvider();
            return serviceProvier;
        }
    }
}
