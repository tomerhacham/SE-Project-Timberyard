using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using ETLTests;
using Microsoft.Extensions.DependencyInjection;
using ETL.Repository;
namespace ETL.Tests
{
    public class RepositoryTests :TestSuit
    {
        [Fact()]
        public void InsertLogTest_CommunicationFail()
        {
            var serviceProvider = ConfigureServices("FaultDBSettings");
            var fileManager = serviceProvider.GetService<FileManager>();
            var deserializer = serviceProvider.GetService<Deserializer>();
            var repository = serviceProvider.GetService<Repository.Repository>();

            var filesResult = fileManager.GetNewLogs();
            if (filesResult.Status)
            {
                var file = filesResult.Data[0];
                var dataResult = fileManager.ReadFromFile(file);
                if (dataResult.Status)
                {
                    var logResult = deserializer.Deserialize(dataResult.Data);
                    Assert.True(logResult.Status);
                    var insertResult = repository.InsertLog(logResult.Data);
                    Assert.False(insertResult.Status);
                    Assert.Contains(file, fileManager.GetNewLogs().Data);
                }
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}