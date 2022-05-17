using ETL.DataObjects;
using ETL.Utils;
using ETL.Utils.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TimberyardClient.Client;

namespace ETL
{
    class Program
    {
        static void Main(string[] args)
        {

            var serviceProvier = ConfigureServices();
            var repository = serviceProvier.GetService<Repository.Repository>();
            var fileManager = serviceProvier.GetService<FileManager>();
            var deserializer = serviceProvier.GetService<Deserializer>();
            var restClient = serviceProvier.GetService<ITimberyardClient>();
            Task.Delay(TimeSpan.FromSeconds(30)).Wait();

            #region Continuation Passing Style
            fileManager.GetNewLogs().ContinueWith((string[] files) =>
            {
                foreach (var file in files)
                {
                    fileManager.ReadFromFile(file).ContinueWith(
                        (string data) =>
                        {
                            deserializer.Deserialize(data).ContinueWith(
                                success: (Log log) =>
                                {
                                    var res = repository.InsertLog(log);
                                    res.ContinueWith(async (bool insertSucceed) =>
                                    {
                                        fileManager.MoveToHandeledLogsDirectory(file);
                                        restClient.CheckAlarmsCondition().Wait();
                                    });
                                }, fail: (Log log) => { fileManager.MoveToFaultLogsDirectory(file); }

                                );
                        });
                }
            });
            #endregion
        }
        /// <summary>
        /// Utility function to build service provider for dependency injection
        /// </summary>
        /// <returns></returns>
        private static ServiceProvider ConfigureServices()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting("ETL").Build();
            var serviceProvier = new ServiceCollection()
                .Configure<Directories>(config.GetSection("Directories"))
                .Configure<DatabaseSettings>(config.GetSection("DatabaseSettings"))
                .AddSingleton<ITimberyardClient>(sp => new TimberyardClient.Client.TimberyardClient(config.GetSection("UserCredentials").Get<UserCredentials>(), config.GetSection("ServiceSettings").Get<ServiceSettings>()))
                .AddSingleton<ILogger>(sp => new Logger("ETL-process"))
                .AddSingleton<FileManager>()
                .AddSingleton<Deserializer>()
                .AddSingleton<Repository.Repository>()
                .BuildServiceProvider();
            return serviceProvier;
        }
    }
}
