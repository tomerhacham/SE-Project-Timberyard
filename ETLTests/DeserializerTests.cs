using Xunit;
using ETL;
using System;
using System.Collections.Generic;
using System.Text;
using ETLTests;
using ETL.DataObjects;
using Microsoft.Extensions.DependencyInjection;
namespace ETL.Tests
{
    public class DeserializerTests : TestSuit
    {
        [Fact()]
        public void DeserializeTest_ValidFile()
        {
            var serviceProvider = ConfigureServices("Valid");
            var fileManager = serviceProvider.GetService<FileManager>();
            var deserializer = serviceProvider.GetService<Deserializer>();

            var filesResult = fileManager.GetNewLogs();
            if (filesResult.Status)
            {
                var dataResult = fileManager.ReadFromFile(filesResult.Data[0]);
                if (dataResult.Status)
                {
                    var logResult = deserializer.Deserialize(dataResult.Data);
                    Assert.True(logResult.Status);
                    Assert.IsType<Log>(logResult.Data);
                }
            }
            else
            {
                Assert.True(false);
            }
        }
        [Fact()]
        public void DeserializeTest_NonValidFile()
        {
            var serviceProvider = ConfigureServices("FaultLog");
            var fileManager = serviceProvider.GetService<FileManager>();
            var deserializer = serviceProvider.GetService<Deserializer>();

            var filesResult = fileManager.GetNewLogs();
            if (filesResult.Status)
            {
                var dataResult = fileManager.ReadFromFile(filesResult.Data[0]);
                if (dataResult.Status)
                {
                    var logResult = deserializer.Deserialize(dataResult.Data);
                    Assert.False(logResult.Status);
                }
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}