using ETL.Utils.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;
namespace ETL.Tests
{
    [Trait("Category", "LoadTest")]
    public class ETLProcessLoadTest
    {
        [Fact]
        public async void LoadTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting("LoadtestSettings").Build();
            var directories = config.GetSection("Directories").Get<Directories>();
            var totalTime = new Stopwatch();
            var processTimer = new Stopwatch();
            totalTime.Start();
            while (totalTime.Elapsed < TimeSpan.FromMinutes(15))
            {
                processTimer.Restart();
                Program.StartETL(profile: "LoadtestSettings", restCalls: false);
                processTimer.Stop();
                var loopResult = Parallel.ForEach(GetAllLogs(directories.HandeledLogsDirectory), file => MoveFileToDirectory(file, directories.NewLogsDirectory));
                while (!loopResult.IsCompleted) { await Task.Delay(100); }
                Assert.True(processTimer.Elapsed < TimeSpan.FromMinutes(5));
                await Task.Delay(TimeSpan.FromMinutes(5).Subtract(processTimer.Elapsed));
            }
            totalTime.Stop();
        }

        private string[] GetAllLogs(string directory)
        {
            var files = Directory.GetFiles(directory, "*.json", searchOption: SearchOption.AllDirectories);
            return files;
        }
        private bool MoveFileToDirectory(string filePath, string directory)
        {
            var dst = Path.Combine(directory, Path.GetFileName(filePath));
            File.Move(filePath, dst);
            return true;
        }
    }
}