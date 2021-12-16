using ETL.DataObjects;
using ETL.Utils;
using ETL.Utils.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace ETL
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting("ETL").Build();
            var directories = config.GetSection("Directories").Get<Directories>();
            var databaseSettings = config.GetSection("DatabaseSettings").Get<DatabaseSettings>();
            var logger = new Logger("ETL-process");
            var repository = new Repository.Repository(databaseSettings,logger);
            var fileManager = new FileManager(directories,logger);
            var deserializer = new Deserializer(logger);

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
                                    res.ContinueWith((bool insertSucceed) => { fileManager.MoveToHandeledLogsDirectory(file); });
                                }, fail: (Log log) => { fileManager.MoveToFaultLogsDirectory(file); }

                        );

                        });
                }
            }); 
            #endregion
        }
    }
}
