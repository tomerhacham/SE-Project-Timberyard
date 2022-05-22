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
            Client.Authenticate().Wait(); //Login as admin

        }

        [Theory]
        [InlineData("admin@timberyard.rbbn.com", "Password!123", HttpStatusCode.OK, true)]
        [InlineData("admin@timberyard.rbbn.com", "NotPassword!123", HttpStatusCode.OK, false)]
        [InlineData("regularUser@timberyard.rbbn.com", "Password!123", HttpStatusCode.OK, true)]
        [InlineData("regularUser@timberyard.rbbn.com", "NotPassword!123", HttpStatusCode.OK, false)]
        [InlineData("regularUser@timberyard.rbbn.com", "", HttpStatusCode.BadRequest, false)]
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
            //Removing the user
            var removeResponse = await Client.RemoveUser(email);
            Assert.NotNull(removeResponse);
            Assert.Equal(expectedStatusCode, removeResponse.StatusCode);
        }
        [Theory]
        [InlineData("nonvalidAdmintimberyard.rbbn.com", "oldPassword", "thisIsTheNewPassword")]
        [InlineData("nonvalidAdmin@timberyard.rbbn.com", "", "thisIsTheNewPassword")]
        [InlineData("nonvalidAdmint@imberyard.rbbn.com", "oldPassword", "")]
        public async Task ChangeSystemAdminPassword_InvalidInput(string email, string oldPassword, string newPassword)
        {
            var changeResponse = await Client.ChangeSystemAdminPassword(email, newPassword, oldPassword);
            Assert.NotNull(changeResponse);
            Assert.Equal(HttpStatusCode.BadRequest, changeResponse.StatusCode);
        }

        [Fact]
        public async Task ChangeSystemAdminPassowrd_ValidFlow()
        {
            var client = Client as TimberyardClient.Client.TimberyardClient;
            var newPassword = "newPassword";

            // Change password
            var changeResponse = await Client.ChangeSystemAdminPassword(client.UserCredentials.Email, newPassword, client.UserCredentials.Password);
            Assert.NotNull(changeResponse);
            Assert.Equal(HttpStatusCode.OK, changeResponse.StatusCode);

            //Attempt to login
            var loginResponse = await Client.Login(client.UserCredentials.Email, newPassword);
            Assert.NotNull(loginResponse);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var token = JsonConvert.DeserializeObject<JWTToken>(loginResponse.Content);

            //Verify token is not empty
            Assert.False(string.IsNullOrEmpty(token.Token));

            // revert changes
            // Change password
            var _changeResponse = await Client.ChangeSystemAdminPassword(client.UserCredentials.Email, client.UserCredentials.Password, newPassword);
            Assert.NotNull(_changeResponse);
            Assert.Equal(HttpStatusCode.OK, _changeResponse.StatusCode);

            //Attempt to login
            var _loginResponse = await Client.Login(client.UserCredentials.Email, newPassword);
            Assert.NotNull(_loginResponse);
            Assert.Equal(HttpStatusCode.OK, _loginResponse.StatusCode);

            var _token = JsonConvert.DeserializeObject<JWTToken>(_loginResponse.Content);
            //Verify token is not empty
            Assert.False(string.IsNullOrEmpty(_token.Token));

        }

        [Theory]
        [InlineData("")]
        [InlineData("this is not valid email")]
        [InlineData("thisIsNotValid.com")]
        [InlineData("thisIsNotValid@comcom")]
        [InlineData("thisIsNotValid@.com")]
        public async Task AddSystemAdmin_InvalidInput(string email)
        {
            var addAdminResponse = await Client.AddSystemAdmin(email);
            Assert.Equal(HttpStatusCode.BadRequest, addAdminResponse.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("this is not valid email")]
        [InlineData("thisIsNotValid.com")]
        [InlineData("thisIsNotValid@comcom")]
        [InlineData("thisIsNotValid@.com")]
        public async Task ForgotPassword_InvalidInput(string email)
        {
            var addAdminResponse = await Client.ForgetPassword(email);
            Assert.Equal(HttpStatusCode.BadRequest, addAdminResponse.StatusCode);
        }
    }
}
