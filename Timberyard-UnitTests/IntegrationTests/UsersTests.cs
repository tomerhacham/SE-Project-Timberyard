using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Services;
using WebService.Utils;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Timberyard_UnitTests.Stubs;
using WebService.Domain.DataAccess;
using System.Linq;
using ETL.DataObjects;
using Microsoft.Extensions.Configuration;

namespace Timberyard_UnitTests.IntegrationTests
{
    public class UsersTests : TestSuit
    {
        Mock<ISMTPClient> SmtpClient { get; set; }
        AlarmsController AlarmsController { get; set; }
        IAlarmsRepository Repository { get; set; }
        public UsersTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            Repository = serviceProvider.GetService<IAlarmsRepository>();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void dummyTest()
        {
            var result = await Repository.GetUserRecord("tomer@post.bgu.ac.il");

        }



    }
}

