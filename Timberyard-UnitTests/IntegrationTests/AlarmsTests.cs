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

namespace Timberyard_UnitTests.IntegrationTests
{
    public class AlarmsTests : TestSuit
    {
        Mock<ISMTPClient> SmtpClient { get; set; }
        AlarmsController AlarmsController { get; set; }
        InMemoryAlarmRepository AlarmsRepository { get; set; }
        public AlarmsTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            AlarmsController = serviceProvider.GetService<AlarmsController>();
            AlarmsRepository = serviceProvider.GetService<IAlarmsRepository>() as InMemoryAlarmRepository;
        }

        [Fact]
        public async void AddNewAlarmTest()
        {
            var result = await AlarmsController.AddNewAlarm("TestAlarm", Field.Catalog, "TestCatalog", 15, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.NotEmpty(AlarmsRepository.Data);
            Assert.Single(AlarmsRepository.Data);
            Assert.Equal("TestAlarm", AlarmsRepository.Data.Values.First().Name);
            Assert.Equal("TestCatalog", AlarmsRepository.Data.Values.First().Objective);
        }

        [Fact]
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
            Assert.Single(AlarmsRepository.Data);
            var inDataAlarm = AlarmsRepository.Data.Values.First();
            Assert.Equal("Edited", inDataAlarm.Name);
            Assert.Equal("Edited", inDataAlarm.Objective);
            Assert.Equal(false, inDataAlarm.Active);
            Assert.Equal(new List<string>() { "tomer@tests.com" }, inDataAlarm.Receivers);
        }

        [Fact]
        public async void RemoveAlarmTest()
        {
            int totalAlarms = 3;
            for (int i = 1; i <= totalAlarms; i++)
            {
                var alarmToInsert = new Alarm($"TestAlarm{i}", Field.Catalog, "TestCatalog", i, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
                alarmToInsert.Id = i;
                var insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert);
                Assert.True(insertionResult.Status);
                Assert.Equal(i,AlarmsRepository.Data.Count);
            }
            var alarmToDelete = new Alarm("Discard", Field.Catalog, "Discard", 1, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            alarmToDelete.Id = 1;
            var removeResult = await AlarmsController.RemoveAlarm(alarmToDelete);
            Assert.True(removeResult.Status);
            Assert.Equal(totalAlarms - 1, AlarmsRepository.Data.Count);
            Assert.False(AlarmsRepository.Data.TryGetValue(removeResult.Data.Id, out Alarm alarm));
        }
    }
}
