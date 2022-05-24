using ETL.Repository.DTO;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Timberyard_UnitTests.Stubs;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.UnitTests
{
    [Trait("Category", "Unit")]
    public class AlarmTests : TestSuit
    {
        Mock<ISMTPClient> SmtpClient { get; set; }
        AlarmsController AlarmsController { get; set; }
        InMemoryAlarmsAndUsersRepository AlarmsRepository { get; set; }
        InMemoryLogsAndTestsRepository LogsAndTestsRepository { get; set; }

        public AlarmTests()
        {
            SmtpClient = new Mock<ISMTPClient>();
            SmtpClient.Setup(client => client.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
               .ReturnsAsync(new Result<string>(true, "Email sent", ""));

            var serviceProvider = ConfigureServices("UnitTests", inMemoryAlarmsRepository: true, inMemoryLogsAndTestRepository: true);
            AlarmsController = serviceProvider.GetService<AlarmsController>();
            AlarmsRepository = serviceProvider.GetService<InMemoryAlarmsAndUsersRepository>() as InMemoryAlarmsAndUsersRepository;
            LogsAndTestsRepository = serviceProvider.GetService<ILogsAndTestsRepository>() as InMemoryLogsAndTestsRepository;
        }

        #region CRUD Alarms Scenarios
        [Theory]
        [InlineData(true, "TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]       // Happy : There are X records of the inputs out of Y records 
        [InlineData(false, "TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "InvalidEmail", "raz@tests.com" })]         // Sad : List of receivers contains an invalid email 
        [InlineData(false, "TestAlarm", Field.Catalog, "TestCatalog", -10, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]     // Bad : Negative threshold
        public async void AddAlarmTest(bool expectedResult, string name, Field field, string objective, int threshold, string[] receivers)
        {
            Result<Alarm> response = await AlarmsController.AddNewAlarm(name, field, objective, threshold, new List<string>(receivers));
            Assert.Equal(expectedResult, response.Status);

            if (response.Status)
            {
                Alarm alarm = response.Data;

                Assert.Equal(name, alarm.Name);
                Assert.Equal((int)field, (int)alarm.Field);
                Assert.Equal(objective, alarm.Objective);
                Assert.Equal(threshold, alarm.Threshold);
                Assert.True(alarm.Active);
                Assert.Equal(receivers, alarm.Receivers);
            }
        }

        [Theory]
        [InlineData(true, "TestEditAlarm", Field.Catalog, "TestCatalog", 15, false, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                                  // Happy : There are X records of the inputs out of Y records 
        [InlineData(false, "TestEditAlarm", Field.Catalog, "TestCatalog", 15, true, new string[] { "tomer@tests.com", "zoe@test.com", "InvalidEmail", "raz@tests.com" })]                             // Sad : List of receivers contains an invalid email 
        [InlineData(false, "TestEditAlarm", Field.Catalog, "TestCatalog", -10, true, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                         // Bad : Negative threshold
        public async void EditAlarmTest(bool expectedResult, string name, Field field, string objective, int threshold, bool active, string[] receivers)
        {
            // Add a valid alarm to DB then try to edit it according to the scenarios 
            Result<Alarm> addAlarmResponse = await AlarmsController.AddNewAlarm(name, field, objective, 5, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Alarm alarmToEdit = addAlarmResponse.Data;
            alarmToEdit.Name = name;
            alarmToEdit.Field = field;
            alarmToEdit.Objective = objective;
            alarmToEdit.Threshold = threshold;
            alarmToEdit.Active = active;
            alarmToEdit.Receivers = new List<string>(receivers);

            Result<Alarm> editAlarmResponse = await AlarmsController.EditAlarm(alarmToEdit);
            Assert.Equal(expectedResult, editAlarmResponse.Status);

            if (editAlarmResponse.Status)
            {
                Alarm alarm_result = editAlarmResponse.Data;

                Assert.Equal(name, alarm_result.Name);
                Assert.Equal((int)field, (int)alarm_result.Field);
                Assert.Equal(objective, alarm_result.Objective);
                Assert.Equal(threshold, alarm_result.Threshold);
                Assert.Equal(active, alarm_result.Active);
                Assert.Equal(receivers, alarm_result.Receivers);
            }
        }

        #endregion

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
