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

namespace Timberyard_UnitTests.IntegrationTests
{
    public class AlarmsTests : TestSuit
    {
        AlarmsController AlarmsController { get; set; }
        InMemoryAlarmsAndUsersRepository AlarmsRepository { get; set; }
        InMemoryLogsAndTestsRepository LogsAndTestsRepository { get; set; }
        public AlarmsTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests", inMemoryAlarmsRepository: true, inMemoryLogsAndTestRepository: true);
            AlarmsController = serviceProvider.GetService<AlarmsController>();
            AlarmsRepository = serviceProvider.GetService<IAlarmsAndUsersRepository>() as InMemoryAlarmsAndUsersRepository;
            LogsAndTestsRepository = serviceProvider.GetService<ILogsAndTestsRepository>() as InMemoryLogsAndTestsRepository;
        }

        #region CRUD Alarms Scenarios
        [Fact]
        [Trait("Category", "Integration")]
        public async void AddNewAlarmTest()
        {
            var result = await AlarmsController.AddNewAlarm("TestAlarm", Field.Catalog, "TestCatalog", 15, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.NotEmpty(AlarmsRepository.Alarms);
            Assert.Single(AlarmsRepository.Alarms);
            Assert.Equal("TestAlarm", AlarmsRepository.Alarms.Values.First().Name);
            Assert.Equal("TestCatalog", AlarmsRepository.Alarms.Values.First().Objective);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void EditAlarmTest()
        {
            var alarmToInsert = new Alarm("TestAlarm", Field.Catalog, "TestCatalog", 15, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            alarmToInsert.Id = 1;
            var insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert);
            Assert.True(insertionResult.Status);
            var originalAlarm = insertionResult.Data;

            //Same Id but different object due to in memory managment
            var clonedAlarm = new Alarm("Edited", Field.Station, "Edited", 1, false, new List<string>() { "tomer@tests.com" });
            clonedAlarm.Id = 1;
            var editResult = await AlarmsController.EditAlarm(clonedAlarm);

            Assert.True(editResult.Status);
            Assert.Single(AlarmsRepository.Alarms);
            var inDataAlarm = AlarmsRepository.Alarms.Values.First();
            Assert.Equal("Edited", inDataAlarm.Name);
            Assert.Equal("Edited", inDataAlarm.Objective);
            Assert.Equal(false, inDataAlarm.Active);
            Assert.Equal(new List<string>() { "tomer@tests.com" }, inDataAlarm.Receivers);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void RemoveAlarmTest()
        {
            int totalAlarms = 3;
            for (int i = 1; i <= totalAlarms; i++)
            {
                var alarmToInsert = new Alarm($"TestAlarm{i}", Field.Catalog, "TestCatalog", i, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
                alarmToInsert.Id = i;
                var insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert);
                Assert.True(insertionResult.Status);
                Assert.Equal(i, AlarmsRepository.Alarms.Count);
            }
            var alarmToDelete = new Alarm("Discard", Field.Catalog, "Discard", 1, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            alarmToDelete.Id = 1;
            var removeResult = await AlarmsController.RemoveAlarm(alarmToDelete);
            Assert.True(removeResult.Status);
            Assert.Equal(totalAlarms - 1, AlarmsRepository.Alarms.Count);
            Assert.False(AlarmsRepository.Alarms.TryGetValue(removeResult.Data.Id, out Alarm alarm));
        }

        #endregion

        #region Check Alarm Condition Scenarios
        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_NoActiveAlarms()
        {
            // Inserting log from the last 24 hours
            // Notice there is no alarm in the database, hence we expect to zero alarms to be set on
            LogsAndTestsRepository.Data.Add(1, new Log() { Date = DateTime.Now });
            var activatedAlarms = await AlarmsController.CheckForAlarmsCondition();
            Assert.Equal(0, activatedAlarms);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_NoRecordsInLast24Hours()
        {
            // There is active alarm but no record inserted
            var alarmToInsert = new Alarm("TestAlarm", Field.Catalog, "TestCatalog", 1, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            alarmToInsert.Id = 1;
            var insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert);
            Assert.True(insertionResult.Status);
            var activatedAlarms = await AlarmsController.CheckForAlarmsCondition();
            Assert.Equal(0, activatedAlarms);
        }
        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_DidNotReachedThreshold()
        {
            // There is active alarm but not enough records to reach the threshold
            var alarmToInsert = new Alarm("TestAlarm", Field.Catalog, "TestCatalog", 2, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            alarmToInsert.Id = 1;
            var insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert);
            Assert.True(insertionResult.Status);
            LogsAndTestsRepository.Data.Add(1, new Log() { Catalog = "TestCatalog", FinalResult = "FAIL", Date = DateTime.Now });
            var activatedAlarms = await AlarmsController.CheckForAlarmsCondition();
            Assert.Equal(0, activatedAlarms);
        }
        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_ReachingThreshold()
        {
            // There is active alarm but not enough records to reach the threshold
            var alarmToInsert = new Alarm("TestAlarm", Field.Catalog, "TestCatalog", 1, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            alarmToInsert.Id = 1;
            var insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert);
            Assert.True(insertionResult.Status);
            LogsAndTestsRepository.Data.Add(1, new Log() { Catalog = "TestCatalog", FinalResult = "FAIL", Date = DateTime.Now });
            var activatedAlarms = await AlarmsController.CheckForAlarmsCondition();
            Assert.Equal(1, activatedAlarms);
        }
        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_2AlarmsReachingThreshold()
        {
            // There is active alarm but not enough records to reach the threshold
            var alarmToInsert = new Alarm("TestAlarm", Field.Catalog, "TestCatalog", 1, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" }) { Id = 1 };
            var alarmToInsert2 = new Alarm("TestAlarm2", Field.Station, "TestStation", 1, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" }) { Id = 2 };
            var insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert);
            Assert.True(insertionResult.Status);
            insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert2);
            Assert.True(insertionResult.Status);
            LogsAndTestsRepository.Data.Add(1, new Log() { Catalog = "TestCatalog", Station = "TestStation", FinalResult = "FAIL", Date = DateTime.Now });
            var activatedAlarms = await AlarmsController.CheckForAlarmsCondition();
            Assert.Equal(2, activatedAlarms);
        }
        #endregion
    }
}

