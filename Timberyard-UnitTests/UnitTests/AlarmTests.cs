using ETL.Repository.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Services;
using Xunit;

namespace Timberyard_UnitTests.UnitTests
{
    public class AlarmTests
    {
        Mock<ISMTPClient> LoggerMock { get; set; }

        public AlarmTests()
        {
            LoggerMock = new Mock<ISMTPClient>();
            LoggerMock = RepositoryMock.Setup(repository => repository.ExecuteQuery(It.IsAny<CardYield>()))
               .ReturnsAsync(new Result<List<dynamic>>(true, new List<dynamic>(), ""));
        }

        [Fact]
        public async void CheckConditionTestCatalog()
        {
            var alarm = new Alarm("Test alarm", Field.Catalog, "TestCatalog", 1, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO(){Catalog="TestCatalog",FinalResult="FAIL"}
            };
            var alarmResult = await alarm.CheckCondition(lastRecords,);
            Assert.True(alarmResult);
        }
    }
}
