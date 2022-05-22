using AcceptanceTests.Utils;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using Xunit;
using Field = TimberyardClient.Client.Field;

namespace AcceptanceTests.UseCases.AlarmsRelated
{
    [Trait("Category", "Acceptance")]
    public class AlarmsTests : TimberyardTestCase
    {
        public AlarmsTests() : base()
        {
            Client.Authenticate().Wait();
        }

        #region CRUD Alarms Scenarios

        [Theory]
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

    }
}
