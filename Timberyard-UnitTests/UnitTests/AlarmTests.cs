using ETL.Repository.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Services;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.UnitTests
{
    public class AlarmTests
    {
        Mock<ISMTPClient> SmtpClient { get; set; }

        public AlarmTests()
        {
            SmtpClient = new Mock<ISMTPClient>();
            SmtpClient.Setup(client => client.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
               .ReturnsAsync(new Result<string>(true, "Email sent", ""));
        }

        #region Catalog
        [Fact]
        public async void CheckConditionTestCatalog_RaiseCondition()
        {
            var alarm = new Alarm("Test alarm", Field.Catalog, "TestCatalog", 1, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO(){Catalog="TestCatalog",FinalResult="FAIL"}
            };
            var alarmResult = await alarm.CheckCondition(lastRecords, SmtpClient.Object);
            Assert.True(alarmResult);
        }

        [Fact]
        public async void CheckConditionTestCatalog_FalseCondition_DifferentCatalog()
        {
            var alarm = new Alarm("Test alarm", Field.Catalog, "TestCatalog", 1, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO() { Catalog = "DifferentCatalog", FinalResult = "FAIL" }

            };
            var alarmResult = await alarm.CheckCondition(lastRecords, SmtpClient.Object);
            Assert.False(alarmResult);
        }

        [Fact]
        public async void CheckConditionTestCatalog_FalseCondition_PassTests()
        {
            var alarm = new Alarm("Test alarm", Field.Catalog, "TestCatalog", 1, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO() { Catalog = "TestCatalog", FinalResult = "PASS" }

            };
            var alarmResult = await alarm.CheckCondition(lastRecords, SmtpClient.Object);
            Assert.False(alarmResult);
        }


        [Fact]
        public async void CheckConditionTestCatalog_FalseCondition_NotReachThreshold()
        {
            var alarm = new Alarm("Test alarm", Field.Catalog, "TestCatalog", 2, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO() { Catalog = "TestCatalog", FinalResult = "FAIL" }

            };
            var alarmResult = await alarm.CheckCondition(lastRecords, SmtpClient.Object);
            Assert.False(alarmResult);
        }
        #endregion
        #region Station

        [Fact]
        public async void CheckConditionTestStation_RaiseCondition()
        {
            var alarm = new Alarm("Test alarm", Field.Station, "TestStation", 1, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO(){Station="TestStation",FinalResult="FAIL"}
            };
            var alarmResult = await alarm.CheckCondition(lastRecords, SmtpClient.Object);
            Assert.True(alarmResult);
        }

        [Fact]
        public async void CheckConditionTestStation_FalseCondition_DifferentCatalog()
        {
            var alarm = new Alarm("Test alarm", Field.Station, "TestStation", 1, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO() { Station = "DifferentStation", FinalResult = "FAIL" }

            };
            var alarmResult = await alarm.CheckCondition(lastRecords, SmtpClient.Object);
            Assert.False(alarmResult);
        }

        [Fact]
        public async void CheckConditionTestStation_FalseCondition_PassTests()
        {
            var alarm = new Alarm("Test alarm", Field.Station, "TestStation", 1, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO() { Station = "TestStation", FinalResult = "PASS" }

            };
            var alarmResult = await alarm.CheckCondition(lastRecords, SmtpClient.Object);
            Assert.False(alarmResult);
        }


        [Fact]
        public async void CheckConditionTestStation_FalseCondition_NotReachThreshold()
        {
            var alarm = new Alarm("Test alarm", Field.Station, "TestStation", 2, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO() { Station = "TestStation", FinalResult = "FAIL" }

            };
            var alarmResult = await alarm.CheckCondition(lastRecords, SmtpClient.Object);
            Assert.False(alarmResult);
        } 
        #endregion
    }
}
