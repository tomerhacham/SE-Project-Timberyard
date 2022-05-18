using AcceptanceTests.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimberyardClient.Client;
using Xunit;

namespace AcceptanceTests.UseCases.AuthenticationRelated
{
    [Trait("Category", "Acceptance")]
    public class UserManagement : TimberyardTestCase
    {
        public UserManagement() : base()
        {
        }

        [Theory]
        [InlineData("admin@timberyard.rbbn.com", "Password!123", HttpStatusCode.OK, true)]
        [InlineData("admin@timberyard.rbbn.com", "NotPassword!123", HttpStatusCode.OK, false)]
        [InlineData("regularuser@timberyard.rbbn.com", "Password!123", HttpStatusCode.OK, true)]
        [InlineData("regularuser@timberyard.rbbn.com", "NotPassword!123", HttpStatusCode.OK, false)]
        [InlineData("regularuser@timberyard.rbbn.com", "", HttpStatusCode.BadRequest, false)]
        [InlineData("nonValidEmailTimberyard.rbbn.com", "asd", HttpStatusCode.BadRequest, false)]
        [InlineData("nonValidEmailTimberyard.rbbn.com", "", HttpStatusCode.BadRequest, false)]
        public async Task LoginTest(string email, string password, HttpStatusCode expectedStatusCode, bool expectedTokenResult)
        {
            var response = await Client.Login(email, password);
            Assert.NotNull(response);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            if (expectedStatusCode.Equals(HttpStatusCode.OK))
            {
                var token = JsonConvert.DeserializeObject<JWTToken>(response.Content);
                Assert.Equal(expectedTokenResult, !string.IsNullOrEmpty(token.Token));
            }
        }

        [Theory]
        [InlineData("validUser@timberyard.rbbn.com", HttpStatusCode.OK, true)]
        [InlineData("nonValidUsertimberyard.rbbn.com", HttpStatusCode.BadRequest, false)]       // email is not valid
        [InlineData("validUser@timberyard.rbbn.com", HttpStatusCode.OK, false)]                 //user is already exists

        public async Task AddUserTests(string email, HttpStatusCode expectedStatusCode, bool expectedResult)
        {
            await Client.Authenticate(); //Login as admin
            var response = await Client.AddUser(email);
            Assert.NotNull(response);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            if (expectedStatusCode.Equals(HttpStatusCode.OK))
            {
                var result = JsonConvert.DeserializeObject<bool>(response.Content);
                Assert.Equal(expectedResult, result);
            }
        }

        [Fact]
        public async Task RemoveUserTests_ValidScenario()
        {
            var emailToAdd = "IHopeThisWontBeAnUser@timberyard.rbbn.com";
            await Client.Authenticate(); //Login as admin

            //Adding a user
            var addResponse = await Client.AddUser(emailToAdd);
            Assert.NotNull(addResponse);
            Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
            var addResult = JsonConvert.DeserializeObject<bool>(addResponse.Content);
            Assert.True(addResult);

            //Removing the user
            var removeResponse = await Client.RemoveUser(emailToAdd);
            Assert.NotNull(removeResponse);
            Assert.Equal(HttpStatusCode.OK, removeResponse.StatusCode);
            var removeResult = JsonConvert.DeserializeObject<bool>(removeResponse.Content);
            Assert.True(removeResult);

            //Attempt to login but should fail
            var loginResponse = await Client.Login(emailToAdd, "password!");
            Assert.NotNull(loginResponse);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var loginResult = JsonConvert.DeserializeObject<bool>(loginResponse.Content);
            Assert.False(loginResult);

        }

        [Fact]
        public async Task RemoveUserTests_EmailNotExists()
        {
            var email = "EmailThatShouldNotExists@timberyard.rbbn.com";
            await Client.Authenticate(); //Login as admin

            //Removing the user
            var removeResponse = await Client.RemoveUser(email);
            Assert.NotNull(removeResponse);
            Assert.Equal(HttpStatusCode.OK, removeResponse.StatusCode);
            var removeResult = JsonConvert.DeserializeObject<bool>(removeResponse.Content);
            Assert.False(removeResult);
        }

        [Theory]
        [InlineData("", HttpStatusCode.BadRequest)]
        [InlineData("                ", HttpStatusCode.BadRequest)]
        [InlineData("notvalid", HttpStatusCode.BadRequest)]
        [InlineData("notvalid.com", HttpStatusCode.BadRequest)]
        [InlineData("notvalid@timber,com", HttpStatusCode.BadRequest)]
        public async Task RemoveUserTests_InputCheck(string email, HttpStatusCode expectedStatusCode)
        {
            await Client.Authenticate(); //Login as admin

            //Removing the user
            var removeResponse = await Client.RemoveUser(email);
            Assert.NotNull(removeResponse);
            Assert.Equal(expectedStatusCode, removeResponse.StatusCode);
        }
    }
}
