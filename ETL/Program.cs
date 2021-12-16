using ETL.DataObjects;
using ETL.Utils;
using ETL.Utils.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ETL
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting("ETL").Build();
            var directories = config.GetSection("Directories").Get<Directories>();
            var databaseSettings = config.GetSection("DatabaseSettings").Get<DatabaseSettings>();

            var repository = new Repository.Repository(databaseSettings);
            var fileManager = new FileManager(directories);
            var deserializer = new Deserializer();

            #region Continuation passing style
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

            #region old Code
            /*            var filesResult = fileManager.GetNewLogs();
                if (filesResult.Status)
                {
                    foreach (var fileName in filesResult.Data)
                    {
                        var readingResult = fileManager.ReadFromFile(fileName);
                        if (readingResult.Status)
                        {
                            var deserializeResult = deserializer.Deserialize(readingResult.Data);
                            if (deserializeResult.Status)
                            {

                                var a = repository.InsertLog(deserializeResult.Data).Result;
                                fileManager.MoveToHandeledLogsDirectory(fileName);
                            }
                            else
                            {
                                LogResult(deserializeResult);
                            }

                        }
                        else { LogResult(readingResult); }
                    }
                }
                else {LogResult(filesResult);}*/
            #endregion

        }

        private static void LogResult<T>(Result<T> result)
        {
            Console.WriteLine(result.Message);
        }
    }
}
