using ETLTests;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
namespace ETL.Tests
{
    [Trait("Category", "Unit")]
    public class FileManagerTests : TestSuit
    {


        /// <summary>
        /// Verify that all the files which been return are json files
        /// </summary>
        [Fact]
        public void GetNewLogsTest_JsonFormat()
        {
            var serviceProvider = ConfigureServices("Valid");
            var fileManager = serviceProvider.GetService<FileManager>();
            var filesResult = fileManager.GetNewLogs();
            Assert.True(filesResult.Status);
            foreach (var file in filesResult.Data)
            {
                Assert.EndsWith(".json", file);
            }

        }

        /// <summary>
        /// Verify searching for new logs is in all the subdirectories
        /// </summary>
        [Fact]
        public void GetNewLogsTest_EmptyParentDirectory()
        {
            var serviceProvider = ConfigureServices("Valid");
            var fileManager = serviceProvider.GetService<FileManager>();
            var filesResult = fileManager.GetNewLogs();
            Assert.True(filesResult.Status);
            Assert.NotEmpty(filesResult.Data);
        }

        /// <summary>
        /// Verify searching for new logs is in all the subdirectories
        /// </summary>
        [Fact]
        public void GetNewLogsTest_DirectoryNotFound()
        {
            var serviceProvider = ConfigureServices("NonExisitngDirectory");
            var fileManager = serviceProvider.GetService<FileManager>();
            var filesResult = fileManager.GetNewLogs();
            Assert.False(filesResult.Status);
        }

        [Fact]
        public void ReadFromFileTest_ValidFile()
        {
            var serviceProvider = ConfigureServices("FaultLog");
            var fileManager = serviceProvider.GetService<FileManager>();
            fileManager.GetNewLogs().ContinueWith(
                success: (data) =>
                {
                    var readingResult = fileManager.ReadFromFile(data[0]);
                    if (readingResult.Status)
                    {
                        Assert.NotEqual(String.Empty, readingResult.Data);
                    }
                    else
                    {
                        Assert.True(false);
                    }
                },
            fail: (data) => Assert.True(false));
        }

        [Fact]
        public void ReadFromFileTest_NonExisitngFile()
        {
            var serviceProvider = ConfigureServices("Valid");
            var fileManager = serviceProvider.GetService<FileManager>();
            var readingResult = fileManager.ReadFromFile("FileNotFound.json");
            Assert.False(readingResult.Status);
        }
    }
}