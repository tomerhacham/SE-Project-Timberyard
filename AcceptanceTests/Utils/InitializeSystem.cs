
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebService.Domain.Business.Queries;
using WebService.Domain.DataAccess;
using WebService.Domain.Interface;
using WebService.Utils;
using WebService.Utils.Models;
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)]


namespace AcceptanceTests.Utils
{
    public class InitializeSystem
    {

        /// <summary>
        /// Utility function to build service provider for dependency injection
        /// </summary>
        /// <returns></returns>
        protected SystemFacade GetSystemFacade()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting("AcceptanceTests").Build();
            var serviceProvier = new ServiceCollection()
                    .Configure<DatabaseSettings>(config.GetSection("DatabaseSettings"))
                    .AddSingleton<ILogger>(sp => new Logger(""))
                    .AddSingleton<LogsAndTestsRepository>()
                    .AddSingleton<QueriesController>().AddSingleton<SystemFacade>().BuildServiceProvider();

            return serviceProvier.GetService<SystemFacade>();
        }
    }

    public static class TestExtensionMethods
    {
        /// <summary>
        /// Reading from the original launchSettings.json under Properties, not from bin\debug
        /// </summary>
        /// <param name="configBuilder"></param>
        /// <param name="workingProfile"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddEnvironmentVariablesForTesting(this IConfigurationBuilder configBuilder, string workingProfile = "testing")
        {
            var filePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty, "Properties", "launchSettings.json");
            if (File.Exists(filePath))
            {
                using var file = File.OpenText(filePath);
                var reader = new JsonTextReader(file);
                var jObject = JObject.Load(reader);

                var profiles = jObject.GetValue("profiles");
                var variables = ((JObject)profiles)
                    ?.GetValue(workingProfile)
                    ?.Children<JProperty>()
                    .Where(prop => prop.Name == "environmentVariables")
                    .SelectMany(prop => prop.Value.Children<JProperty>()).ToList() ?? new List<JProperty>();

                foreach (var variable in variables)
                {
                    System.Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                }
            }

            return configBuilder.AddEnvironmentVariables();
        }
    }
}
