using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Timberyard_UnitTests.Stubs;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Authentication;
using WebService.Domain.Business.Queries;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using WebService.Utils;
using WebService.Utils.Models;

namespace Timberyard_UnitTests
{
    public class TestSuit
    {

        /// <summary>
        /// Utility function to build service provider for dependency injection
        /// </summary>
        /// <returns></returns>
        protected ServiceProvider ConfigureServices(string profile, [Optional] bool inMemoryLogsAndTestRepository, [Optional] bool inMemoryAlarmsRepository)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting(profile).Build();
            var serviceProvier = new ServiceCollection()
                .Configure<DatabaseSettings>(config.GetSection("DatabaseSettings"))
                .Configure<SMPTClientSettings>(config.GetSection("SMPTClientSettings"))

                .AddSingleton<ISMTPClient>((sp) =>
                {
                    var SmtpClient = new Mock<ISMTPClient>();
                    SmtpClient.Setup(client => client.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                            .ReturnsAsync(new Result<string>(true, "Email sent", ""));
                    return SmtpClient.Object;
                });

            if (inMemoryAlarmsRepository)
            {
                serviceProvier.AddSingleton<IAlarmsAndUsersRepository, InMemoryAlarmsAndUsersRepository>();
            }
            else
            {
                serviceProvier.AddSingleton<IAlarmsAndUsersRepository, AlarmsAndUsersRepository>();
            }

            if (inMemoryLogsAndTestRepository)
            {
                serviceProvier.AddSingleton<ILogsAndTestsRepository, InMemoryLogsAndTestsRepository>();
            }
            else
            {
                serviceProvier.AddSingleton<ILogsAndTestsRepository, LogsAndTestsRepository>();
            }

            serviceProvier.AddSingleton<ILogger>(sp => new Logger("IntegrationTesting"))
            .AddSingleton<QueriesController>()
            .AddSingleton<AlarmsController>()
            .AddSingleton<AuthenticationController>();

            return serviceProvier.BuildServiceProvider();
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
