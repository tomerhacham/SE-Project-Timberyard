using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class UsersTests : TestSuit
    {
        Mock<ISMTPClient> SmtpClient { get; set; }
        AlarmsController AlarmsController { get; set; }
        IAlarmsAndUsersRepository Repository { get; set; }
        public UsersTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            Repository = serviceProvider.GetService<IAlarmsAndUsersRepository>();
        }

        [Fact]
        public async void dummyTest()
        {
            var result = await Repository.GetUserRecord("tomer@post.bgu.ac.il");

        }



    }
}

