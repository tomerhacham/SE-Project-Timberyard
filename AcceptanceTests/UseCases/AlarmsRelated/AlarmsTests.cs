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
    public class AlarmsTests : TimberyardTestCase
    {
        public AlarmsTests() : base()
        { }

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

            if(response.StatusCode.Equals(HttpStatusCode.OK))
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
        [InlineData(HttpStatusCode.OK, "TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                                  // Happy : There are X records of the inputs out of Y records 
        [InlineData(HttpStatusCode.BadRequest, "TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "InvalidEmail", "raz@tests.com" })]                             // Sad : List of receivers contains an invalid email 
        [InlineData(HttpStatusCode.BadRequest, "TestAlarm", Field.Catalog, "TestCatalog", -10, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                         // Bad : Negative threshold
        public async void EditAlarmTest(HttpStatusCode expectedStatusCod, string name, Field field, string objective, int threshold, string[] receivers)
        {
            // TODO - add Id to EditAlarm function 
            var alarmToInsert = new Alarm("TestAlarm", Field.Catalog, "TestCatalog", 15, true, new List<string>() { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" });
            alarmToInsert.Id = 1;
            var insertionResult = await AlarmsRepository.InsertAlarm(alarmToInsert);
            Assert.True(insertionResult.Status);
            var originalAlarm = insertionResult.Data;

            IRestResponse response = await Client.EditAlarm(name, field, objective, threshold, new List<string>(receivers));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            FullAlarmModel alarm_result = JsonConvert.DeserializeObject<FullAlarmModel>(response.Content);


        }

        [Theory]
        [Trait("Category", "Acceptance")]
        [InlineData("TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                              // Happy : There are X records of the inputs out of Y records 
        public async void RemoveAlarmTest(string name, Field field, string objective, int threshold, string[] receivers)
        {
            IRestResponse response = await Client.RemoveAlarm(name, field, objective, threshold, new List<string>(receivers));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);

            // ADD CHECKS 
        }

        #endregion


        #region Check Alarm Condition Scenarios

        [Fact]
        [Trait("Category", "Acceptance")]
        public async void CheckForAlarmsConditionTest_NoActiveAlarms()
        {
            // Inserting log from the last 24 hours
            // Notice there is no alarm in the database, hence we expect to zero alarms to be set on

            IRestResponse response = await Client.CheckAlarmsCondition();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);

            // ADD CHECKS 
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async void CheckForAlarmsConditionTest_NoRecordsInLast24Hours()
        { }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async void CheckForAlarmsConditionTest_DidNotReachedThreshold()
        { }

        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_ReachingThreshold()
        { }

        [Fact]
        [Trait("Category", "Integration")]
        public async void CheckForAlarmsConditionTest_2AlarmsReachingThreshold()
        { }

        #endregion

    }
}
