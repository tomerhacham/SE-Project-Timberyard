using AcceptanceTests.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using TimberyardClient.Client;
using Xunit;

namespace AcceptanceTests.UseCases.AlarmsRelated
{
    public class AlarmsTests : TimberyardTestCase
    {
        public AlarmsTests() : base()
        { }

        #region CRUD Alarms Scenarios

        [Theory]
        [Trait("Category", "Acceptance")]
        [InlineData("TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                              // Happy : There are X records of the inputs out of Y records 
        public async void AddAlarmTest(string name, Field field, string objective, int threshold, string[] receivers)
        {
            IRestResponse response = await Client.AddNewAlarm(name, field, objective, threshold, new List<string>(receivers));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);

            // ADD CHECKS 
        }

        [Theory]
        [Trait("Category", "Acceptance")]
        [InlineData("TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                              // Happy : There are X records of the inputs out of Y records 
        public async void EditAlarmTest(string name, Field field, string objective, int threshold, string[] receivers)
        {
            IRestResponse response = await Client.EditAlarm(name, field, objective, threshold, new List<string>(receivers));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);

            // ADD CHECKS 
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
