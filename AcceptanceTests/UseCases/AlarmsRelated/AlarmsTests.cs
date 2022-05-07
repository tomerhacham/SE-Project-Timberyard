using AcceptanceTests.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using TimberyardClient.Client;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Alarms;
using Xunit;
using Field = TimberyardClient.Client.Field;

namespace AcceptanceTests.UseCases.AlarmsRelated
{
    public class AlarmsTests : TimberyardTestCase , IDisposable
    {
        public AlarmsTests() : base()
        { }

        public void Dispose()
        {
            // TODO - Delete all alarms that are in the tests data base
            // TODO - Delete all Logs that where added in these tests 
        }

        #region CRUD Alarms Scenarios

        [Theory]
        [Trait("Category", "Acceptance")]
        [InlineData(HttpStatusCode.OK, "TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                              // Happy : There are X records of the inputs out of Y records 
        [InlineData(HttpStatusCode.BadRequest, "TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "InvalidEmail", "raz@tests.com" })]                              // Sad : List of receivers contains an invalid email 
        [InlineData(HttpStatusCode.BadRequest, "TestAlarm", Field.Catalog, "TestCatalog", -10, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                              // Bad : Negative threshold
        public async void AddAlarmTest(HttpStatusCode expectedStatusCod, string name, Field field, string objective, int threshold, string[] receivers)
        {
            IRestResponse response = await Client.AddNewAlarm(name, field, objective, threshold, new List<string>(receivers));
            Assert.Equal(expectedStatusCod, response.StatusCode);

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                FullAlarmModel alarm_result = JsonConvert.DeserializeObject<FullAlarmModel>(response.Content);

                Assert.Equal(name, alarm_result.Name);
                Assert.Equal((int)field, (int)alarm_result.Field);
                Assert.Equal(objective, alarm_result.Objective);
                Assert.Equal(threshold, alarm_result.Threshold);
                Assert.True(alarm_result.Active);
                Assert.Equal(receivers, alarm_result.Receivers);
            }
        }

        [Theory]
        [Trait("Category", "Acceptance")]
        [InlineData(HttpStatusCode.OK, "TestEditAlarm", Field.Catalog, "TestCatalog", 15, false, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                                  // Happy : There are X records of the inputs out of Y records 
        [InlineData(HttpStatusCode.BadRequest, "TestEditAlarm", Field.Catalog, "TestCatalog", 15, true, new string[] { "tomer@tests.com", "zoe@test.com", "InvalidEmail", "raz@tests.com" })]                             // Sad : List of receivers contains an invalid email 
        [InlineData(HttpStatusCode.BadRequest, "TestEditAlarm", Field.Catalog, "TestCatalog", -10, true, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                         // Bad : Negative threshold
        public async void EditAlarmTest(HttpStatusCode expectedStatusCod, string name, Field field, string objective, int threshold, bool active, string[] receivers)
        {
            // Add a valid alarm to DB then try to edit it according to the scenarios 
            IRestResponse addAlarmResponse = await Client.AddNewAlarm(name, field, objective, 5, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            FullAlarmModel alarm_result = JsonConvert.DeserializeObject<FullAlarmModel>(addAlarmResponse.Content);
            int alarmId = alarm_result.Id;

            IRestResponse editAlarmResponse = await Client.EditAlarm(alarmId, name, field, objective, threshold, active, new List<string>(receivers));
            Assert.Equal(expectedStatusCod, editAlarmResponse.StatusCode);

            if (editAlarmResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                alarm_result = JsonConvert.DeserializeObject<FullAlarmModel>(editAlarmResponse.Content);

                Assert.Equal(name, alarm_result.Name);
                Assert.Equal((int)field, (int)alarm_result.Field);
                Assert.Equal(objective, alarm_result.Objective);
                Assert.Equal(threshold, alarm_result.Threshold);
                Assert.Equal(active, alarm_result.Active);
                Assert.Equal(receivers, alarm_result.Receivers);
            }
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async void RemoveAlarmTest()
        {

            // Add a valid alarm to DB then try to remove it.
            IRestResponse addAlarmResponse = await Client.AddNewAlarm("TestRemoveAlarm", Field.Catalog, "TestCatalog", 5, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            FullAlarmModel alarm_result = JsonConvert.DeserializeObject<FullAlarmModel>(addAlarmResponse.Content);
            int alarmId = alarm_result.Id;

            // Notice - Removing an alarm doesn't have any bad/sad outcomes. Therefore, removing an alarm always succeeds.
            IRestResponse removeAlarmResponse = await Client.RemoveAlarm(alarmId);
            Assert.Equal(HttpStatusCode.OK, removeAlarmResponse.StatusCode);

            // Attempting to remove the alarm again should fail
            IRestResponse AttemptToRemoveAlarmAgainResponse = await Client.RemoveAlarm(alarmId);
            Assert.Equal(HttpStatusCode.BadRequest, AttemptToRemoveAlarmAgainResponse.StatusCode);

            // Editing a non-existing alarm should fail
            IRestResponse AttemptToEditAlarmResponse = await Client.EditAlarm(alarmId, "TestRemoveAlarm", Field.Catalog, "TestCatalog", 5, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.Equal(HttpStatusCode.BadRequest, AttemptToEditAlarmResponse.StatusCode);
        }

        #endregion


        #region Check Alarm Condition Scenarios

        [Fact]
        [Trait("Category", "Acceptance")]
        public async void CheckForAlarmsConditionTest_NoActiveAlarms()
        {
            // add alarm
            IRestResponse alarm_response = await Client.AddNewAlarm("TestAlarm", Field.Catalog, "TestCatalog", 1, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.Equal(HttpStatusCode.OK, alarm_response.StatusCode);
            FullAlarmModel alarm_result = JsonConvert.DeserializeObject<FullAlarmModel>(alarm_response.Content);

            //change active to false
            IRestResponse editAlarmResponse = await Client.EditAlarm(alarm_result.Id, alarm_result.Name, (Field)alarm_result.Field, alarm_result.Objective, alarm_result.Threshold, false, alarm_result.Receivers);
            Assert.Equal(HttpStatusCode.OK, editAlarmResponse.StatusCode);

            // check that the number of active alarms is 0
            IRestResponse response = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            int activatedAlarms = JsonConvert.DeserializeObject<int>(response.Content);
            Assert.Equal(0, activatedAlarms);

            // TODO - add 2 logs with the same catalog as alarm

            // check that the number of active alarms is still 0 although there are logs that satisfy the alarm condition
            IRestResponse responseSecondAttempt = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, responseSecondAttempt.StatusCode);
            int activatedAlarmsSecondAttempt = JsonConvert.DeserializeObject<int>(responseSecondAttempt.Content);
            Assert.Equal(0, activatedAlarmsSecondAttempt);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async void CheckForAlarmsConditionTest_NoRecordsInLast24Hours()
        {
            // add alarm
            IRestResponse alarm_response = await Client.AddNewAlarm("TestAlarm", Field.Catalog, "TestCatalog", 1, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.Equal(HttpStatusCode.OK, alarm_response.StatusCode);

            // TODO - check that there are no logs added in the last 24 hours


            // check that the number of active alarms is 0
            IRestResponse response = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            int activatedAlarms = JsonConvert.DeserializeObject<int>(response.Content);
            Assert.Equal(0, activatedAlarms);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async void CheckForAlarmsConditionTest_DidNotReachedThreshold()
        {
            // add alarm
            IRestResponse alarm_response = await Client.AddNewAlarm("TestAlarm", Field.Catalog, "TestCatalog", 3, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.Equal(HttpStatusCode.OK, alarm_response.StatusCode);
            FullAlarmModel alarm_result = JsonConvert.DeserializeObject<FullAlarmModel>(alarm_response.Content);

            // check that the number of active alarms is 0
            IRestResponse response = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            int activatedAlarms = JsonConvert.DeserializeObject<int>(response.Content);
            Assert.Equal(0, activatedAlarms);

            // TODO - add 2 logs with the same catalog as alarm

            // check that the number of active alarms is 0 because we did not reach threshold
            IRestResponse responseSecondAttempt = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, responseSecondAttempt.StatusCode);
            int activatedAlarmsSecondAttempt = JsonConvert.DeserializeObject<int>(responseSecondAttempt.Content);
            Assert.Equal(0, activatedAlarmsSecondAttempt);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_ReachingThreshold()
        {
            // add alarm
            IRestResponse alarm_response = await Client.AddNewAlarm("TestAlarm", Field.Catalog, "TestCatalog", 2, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.Equal(HttpStatusCode.OK, alarm_response.StatusCode);
            FullAlarmModel alarm_result = JsonConvert.DeserializeObject<FullAlarmModel>(alarm_response.Content);

            // check that the number of active alarms is 0
            IRestResponse response = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            int activatedAlarms = JsonConvert.DeserializeObject<int>(response.Content);
            Assert.Equal(0, activatedAlarms);

            // TODO - add 2 logs with the same catalog as alarm

            // check that the number of active alarms is 1 since we reached threshold
            IRestResponse responseSecondAttempt = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, responseSecondAttempt.StatusCode);
            int activatedAlarmsSecondAttempt = JsonConvert.DeserializeObject<int>(responseSecondAttempt.Content);
            Assert.Equal(1, activatedAlarmsSecondAttempt);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_2AlarmsReachingThreshold()
        {
            // add 2 alarms
            IRestResponse alarm1_response = await Client.AddNewAlarm("TestAlarm1", Field.Catalog, "TestCatalog1", 1, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.Equal(HttpStatusCode.OK, alarm1_response.StatusCode);
            IRestResponse alarm2_response = await Client.AddNewAlarm("TestAlarm2", Field.Catalog, "TestCatalog2", 1, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            Assert.Equal(HttpStatusCode.OK, alarm2_response.StatusCode);

            // check that the number of active alarms is 0
            IRestResponse response = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            int activatedAlarms = JsonConvert.DeserializeObject<int>(response.Content);
            Assert.Equal(0, activatedAlarms);

            // TODO - add 2 logs for each alarm with the same catalog as their alarm

            // check that the number of active alarms are 2 since we reached threshold for each alarm
            IRestResponse responseSecondAttempt = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, responseSecondAttempt.StatusCode);
            int activatedAlarmsSecondAttempt = JsonConvert.DeserializeObject<int>(responseSecondAttempt.Content);
            Assert.Equal(2, activatedAlarmsSecondAttempt);
        }

        #endregion

    }
}
