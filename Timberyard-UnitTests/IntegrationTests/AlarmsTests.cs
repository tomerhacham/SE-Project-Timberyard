using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Services;
using WebService.Utils;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    public class AlarmsTests : TestSuit
    {
        Mock<ISMTPClient> SmtpClient { get; set; }
        AlarmsController AlarmsController { get; set; }
        public AlarmsTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            AlarmsController = serviceProvider.GetService<AlarmsController>();
            AlarmsController.
            SmtpClient = new Mock<ISMTPClient>();
            SmtpClient.Setup(client => client.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
               .ReturnsAsync(new Result<string>(true, "Email sent", ""));
        }
    }
}
