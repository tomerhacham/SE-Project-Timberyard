using AcceptanceTests.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TimberyardClient.Client;
using Xunit;

namespace AcceptanceTests.UseCases.AuthenticationRelated
{
    [Trait("Category", "Acceptance")]

    public class RegularUserPermissions : TimberyardTestCase
    {
        public RegularUserPermissions() : base(new UserCredentials { Email = "regularUser@timberyard.rbbn.com", Password = "Password123!" })
        {
            GetServiceProvider().GetService<DatabaseUtils>().AddOrUpdateRegularUser("regularUser@timberyard.rbbn.com", "Password123!").Wait();
            Client.Authenticate().Wait();
        }
        #region Queries
        [Theory]
        [InlineData("X39337", 2021, 2022, HttpStatusCode.OK)]                              // Happy : There are X records of the inputs out of Y records
        [InlineData("X39337", 2020, 2019, HttpStatusCode.BadRequest)]                     // Bad: invalid dates
        [InlineData("", 2021, 2022, HttpStatusCode.BadRequest)]                           // Bad: empty catalog

        public async void BounderiesPermitted(string catalog, int startDate, int endDate, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.CalculateBoundaries(catalog, new DateTime(startDate, 11, 15), new DateTime(endDate, 01, 22));
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }



        [Theory]
        [InlineData("X93655", 2021, 2022, HttpStatusCode.OK)]                                          // Happy : There are X records of the inputs out of Y records ( where X==Y )
        [InlineData("", 2021, 2022, HttpStatusCode.BadRequest)]                                       // Bad : Invalid catalog           
        [InlineData("X16434", 2022, 2021, HttpStatusCode.BadRequest)]                                // Bad : invalid dates         
        public async void CardTestDurationPermitted(string catalog, int startDate, int endDate, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.CalculateCardTestDuration(catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData("X93655", 2021, 2022, HttpStatusCode.OK)]                              // Happy : There are X records of the inputs out of Y records 
        [InlineData("", 2021, 2022, HttpStatusCode.BadRequest)]                                                                // Bad : invalid catalog         
        [InlineData("X93655", 2022, 2021, HttpStatusCode.BadRequest)]                                                          // Bad : invalid dates   // Bad : invalid dates         
        public async void CardYieldPermitted(string catalog, int startDate, int endDate, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.CalculateCardYield(catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData("9901X_JTAG", 2021, 2022, HttpStatusCode.OK)]
        [InlineData("X93655", 2022, 2021, HttpStatusCode.BadRequest)]                                                                         // Bad: Invalid dates
        [InlineData("", 2021, 2022, HttpStatusCode.BadRequest)]                                                                            // Bad: Invalid catalog
        public async void NFFPermitted(string cardName, int startDate, int endDate, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.CalculateNoFailureFound(cardName, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01), 5 * 60 * 60);
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData("L5", "X93655", 2021, 2022, HttpStatusCode.OK)]                          // Happy : There are X records of the inputs out of Y records
        [InlineData("L5", "X93655", 2022, 2021, HttpStatusCode.BadRequest)]               // Bad: invalid dates
        [InlineData("10B", "", 2021, 2022, HttpStatusCode.BadRequest)]                // Bad: invalid catalog
        [InlineData("", "X16434", 2021, 2022, HttpStatusCode.BadRequest)]             // Bad: invalid station

        public async void StationAndCardYieldPermitted(string station, string catalog, int startDate, int endDate, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.CalculateStationAndCardYield(station, catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Theory]
        [InlineData(2021, 2022, HttpStatusCode.OK)]                                       // Happy: there is data in these dates
        [InlineData(2022, 2021, HttpStatusCode.BadRequest)]                               // Bad: Invalid dates
        public async void StationsYielPermitted(int startDate, int endDate, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.CalculateStationsYield(new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
        [Theory]
        [InlineData(2021, 2022, HttpStatusCode.OK)]
        [InlineData(2022, 2021, HttpStatusCode.BadRequest)]               // Bad: invalid dates
        public async void TesterLoadPermitted(int startDate, int endDate, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.CalculateTesterLoad(new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
        #endregion

        #region Alarms

        [Theory]
        [InlineData(HttpStatusCode.Unauthorized, "TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                              // Happy : There are X records of the inputs out of Y records 
        [InlineData(HttpStatusCode.Unauthorized, "TestAlarm", Field.Catalog, "TestCatalog", 15, new string[] { "tomer@tests.com", "zoe@test.com", "InvalidEmail", "raz@tests.com" })]                              // Sad : List of receivers contains an invalid email 
        [InlineData(HttpStatusCode.Unauthorized, "TestAlarm", Field.Catalog, "TestCatalog", -10, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                              // Bad : Negative threshold
        public async void AddAlarmTestNotPermitted(HttpStatusCode expectedStatusCod, string name, Field field, string objective, int threshold, string[] receivers)
        {
            var response = await Client.AddNewAlarm(name, field, objective, threshold, new List<string>(receivers));
            Assert.Equal(expectedStatusCod, response.StatusCode);
        }
        [Theory]
        [InlineData(HttpStatusCode.Unauthorized, "TestEditAlarm", Field.Catalog, "TestCatalog", 15, false, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                                  // Happy : There are X records of the inputs out of Y records 
        [InlineData(HttpStatusCode.Unauthorized, "TestEditAlarm", Field.Catalog, "TestCatalog", 15, true, new string[] { "tomer@tests.com", "zoe@test.com", "InvalidEmail", "raz@tests.com" })]                             // Sad : List of receivers contains an invalid email 
        [InlineData(HttpStatusCode.Unauthorized, "TestEditAlarm", Field.Catalog, "TestCatalog", -10, true, new string[] { "tomer@tests.com", "zoe@test.com", "shaked@test.com", "raz@tests.com" })]                         // Bad : Negative threshold
        public async void EditAlarmTestNotPermitted(HttpStatusCode expectedStatusCod, string name, Field field, string objective, int threshold, bool active, string[] receivers)
        {
            var editAlarmResponse = await Client.EditAlarm(0, name, field, objective, threshold, active, new List<string>(receivers));
            Assert.Equal(expectedStatusCod, editAlarmResponse.StatusCode);
        }
        [Fact]
        public async void RemoveAlarmTestNotPermitted()
        {
            var removeAlarmResponse = await Client.RemoveAlarm(0);
            Assert.Equal(HttpStatusCode.Unauthorized, removeAlarmResponse.StatusCode);
        }
        #endregion

        #region Authentication

        [Theory]
        [InlineData("validUser@timberyard.rbbn.com", HttpStatusCode.Unauthorized)]
        [InlineData("nonValidUsertimberyard.rbbn.com", HttpStatusCode.Unauthorized)]       // email is not valid
        public async Task AddUserTests(string email, HttpStatusCode expectedStatusCode)
        {
            var response = await Client.AddUser(email);
            Assert.NotNull(response);
            Assert.Equal(expectedStatusCode, response.StatusCode);

        }
        [Fact]
        public async Task RemoveUserTests()
        {
            var clientProxy = Client as TimberyardClientProxy;
            var clientAdapter = clientProxy.RealClient as TimberyardClientRealAdapter;
            var client = clientAdapter.RealClient as TimberyardClient.Client.TimberyardClient;

            //Removing the user
            var removeResponse = await Client.RemoveUser(client.UserCredentials.Email);
            Assert.Equal(HttpStatusCode.Unauthorized, removeResponse.StatusCode);
        }

        [Fact]
        public async Task ChangeSystemAdminPassword()
        {
            var changeResponse = await Client.ChangeSystemAdminPassword("newPassword", "oldPassword");
            Assert.Equal(HttpStatusCode.Unauthorized, changeResponse.StatusCode);
        }

        [Fact]
        public async Task AddSystemAdmin()
        {
            var addAdminResponse = await Client.AddSystemAdmin("someEmail@timberyard.rbbn.com");
            Assert.Equal(HttpStatusCode.Unauthorized, addAdminResponse.StatusCode);
        }

        #endregion
    }
}
