using AcceptanceTests.Utils;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using TimberyardClient.Client;
using Xunit;


namespace AcceptanceTests.UseCases.AuthenticationRelated
{
    [Collection("AdminUserPermission")]
    [CollectionDefinition("AdminUserPermission", DisableParallelization = true)]
    public class AdminUserPermission : TimberyardTestCase
    {
        public AdminUserPermission() : base()
        {
            Client.Authenticate().Wait(); //Login as admin

        }

        [Theory]
        [InlineData("validUser@timberyard.rbbn.com", HttpStatusCode.OK, true)]
        [InlineData("nonValidUsertimberyard.rbbn.com", HttpStatusCode.BadRequest, false)]       // email is not valid

        public async Task AddUserTests(string email, HttpStatusCode expectedStatusCode, bool expectedResult)
        {
            var response = await Client.AddUser(email);
            Assert.NotNull(response);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            if (expectedStatusCode.Equals(HttpStatusCode.OK))
            {
                var result = JsonConvert.DeserializeObject<Result<bool>>(response.Content).Data;
                Assert.Equal(expectedResult, result);

                //Attempt to add the same user again and expect fail
                response = await Client.AddUser(email);
                Assert.NotNull(response);
                Assert.Equal(expectedStatusCode, response.StatusCode);
                result = JsonConvert.DeserializeObject<Result<bool>>(response.Content).Data;
                Assert.False(result);

                //Clean up
                //Removing the user
                var removeResponse = await Client.RemoveUser(email);
                Assert.NotNull(removeResponse);
                Assert.Equal(HttpStatusCode.OK, removeResponse.StatusCode);
                var removeResult = JsonConvert.DeserializeObject<Result<bool>>(removeResponse.Content).Data;
                Assert.True(removeResult);
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
            var addResult = JsonConvert.DeserializeObject<Result<bool>>(addResponse.Content).Data;
            Assert.True(addResult);

            //Removing the user
            var removeResponse = await Client.RemoveUser(emailToAdd);
            Assert.NotNull(removeResponse);
            Assert.Equal(HttpStatusCode.OK, removeResponse.StatusCode);
            var removeResult = JsonConvert.DeserializeObject<Result<bool>>(removeResponse.Content).Data;
            Assert.True(removeResult);

            //Attempt to login but should fail
            var loginResponse = await Client.Login(emailToAdd, "password!");
            Assert.NotNull(loginResponse);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.False(JsonConvert.DeserializeObject<Result<JWTToken>>(loginResponse.Content).Status);
            Assert.Null(JsonConvert.DeserializeObject<Result<JWTToken>>(loginResponse.Content).Data);
        }

        [Fact]
        public async Task RemoveUserTests_EmailNotExists()
        {
            var email = "EmailThatShouldNotExists@timberyard.rbbn.com";

            //Removing the user
            var removeResponse = await Client.RemoveUser(email);
            Assert.NotNull(removeResponse);
            Assert.Equal(HttpStatusCode.OK, removeResponse.StatusCode);
            var removeResult = JsonConvert.DeserializeObject<Result<bool>>(removeResponse.Content).Data;
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
        [InlineData("", "")]
        [InlineData("", "thisIsTheNewPassword")]
        [InlineData("oldPassword", "")]
        public async Task ChangeSystemAdminPassword_InvalidInput(string oldPassword, string newPassword)
        {
            var changeResponse = await Client.ChangeSystemAdminPassword(newPassword, oldPassword);
            Assert.NotNull(changeResponse);
            Assert.Equal(HttpStatusCode.BadRequest, changeResponse.StatusCode);
        }

        [Fact()]
        public async Task ChangeSystemAdminPassword_ValidFlow()
        {
            var clientProxy = Client as TimberyardClientProxy;
            var clientAdapter = clientProxy.RealClient as TimberyardClientRealAdapter;
            var client = clientAdapter.RealClient as TimberyardClient.Client.TimberyardClient;
            var newPassword = "newPassword";

            // Change password
            var changeResponse = await Client.ChangeSystemAdminPassword(newPassword, client.UserCredentials.Password);
            Assert.NotNull(changeResponse);
            Assert.Equal(HttpStatusCode.OK, changeResponse.StatusCode);
            Assert.True((JsonConvert.DeserializeObject<Result<bool>>(changeResponse.Content).Data), "changing password failed");

            //Attempt to login
            var loginResponse = await Client.Login(client.UserCredentials.Email, newPassword);
            Assert.NotNull(loginResponse);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var token = JsonConvert.DeserializeObject<Result<JWTToken>>(loginResponse.Content).Data;

            //Verify token is not empty
            Assert.NotNull(token);
            Assert.NotEmpty(token.Token);

            // revert changes
            // Change password
            var _changeResponse = await Client.ChangeSystemAdminPassword(client.UserCredentials.Password, newPassword);
            Assert.NotNull(_changeResponse);
            Assert.Equal(HttpStatusCode.OK, _changeResponse.StatusCode);

            //Attempt to login
            var _loginResponse = await Client.Login(client.UserCredentials.Email, client.UserCredentials.Password);
            Assert.NotNull(_loginResponse);
            Assert.Equal(HttpStatusCode.OK, _loginResponse.StatusCode);

            var _token = JsonConvert.DeserializeObject<Result<JWTToken>>(_loginResponse.Content).Data;
            //Verify token is not empty
            Assert.NotNull(_token);
            Assert.NotEmpty(_token.Token);
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
    }
}
