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
    public class Program
    {
        public static void Main(string[] args)
        {
            StartETL();
        }

        public static void StartETL(string profile = "ETL", bool restCalls = true)
        {
            var serviceProvier = ConfigureServices(profile);
            var repository = serviceProvier.GetService<Repository.Repository>();
            var fileManager = serviceProvier.GetService<FileManager>();
            var deserializer = serviceProvier.GetService<Deserializer>();
            var restClient = serviceProvier.GetService<ITimberyardClient>();

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
                                    res.ContinueWith((bool insertSucceed) =>
                                    {
                                        fileManager.MoveToHandeledLogsDirectory(file);
                                        if (restCalls)
                                        {
                                            restClient.CheckAlarmsCondition().Wait();
                                        }
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
        public static ServiceProvider ConfigureServices(string profile)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting(profile).Build();
            var serviceProvier = new ServiceCollection()
                .Configure<Directories>(config.GetSection("Directories"))
                .Configure<DatabaseSettings>(config.GetSection("DatabaseSettings"))
                .Configure<UserCredentials>(config.GetSection("UserCredentials"))
                .Configure<ServiceSettings>(config.GetSection("ServiceSettings"))
                .AddSingleton<ITimberyardClient, TimberyardClient.Client.TimberyardClient>().AddSingleton<ILogger>(sp => new Logger("ETL-process"))
                .AddSingleton<FileManager>()
                .AddSingleton<Deserializer>()
                .AddSingleton<Repository.Repository>()
                .BuildServiceProvider();
            return serviceProvier;
        }

    }
}
