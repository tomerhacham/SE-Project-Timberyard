using Xunit;
using ETL;
using System;
using System.Collections.Generic;
using System.Text;
using ETLTests;

namespace ETL.Tests
{
    public class FileManagerTests : TestSuit<FileManager>
    {


        /// <summary>
        /// Verify that all the files which been return are json files
        /// </summary>
        [Fact()]
        public void GetFilesTests_JsonFormat()
        {
            var fileManager = GetConfiguratedComponent("Valid");
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
        [Fact()]
        public void GetFilesTests_EmptyParentDirectory()
        {
            var fileManager = GetConfiguratedComponent("Valid");
            var filesResult = fileManager.GetNewLogs();
            Assert.True(filesResult.Status);
            Assert.NotEmpty(filesResult.Data);
        }

        /// <summary>
        /// Verify searching for new logs is in all the subdirectories
        /// </summary>
        [Fact()]
        public void GetFilesTests_DirectoryNotFound()
        {
            var fileManager = GetConfiguratedComponent("NonExisitngDirectory");
            var filesResult = fileManager.GetNewLogs();
            Assert.False(filesResult.Status);
        }
    }
}