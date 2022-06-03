using AcceptanceTests.Utils;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using TimberyardClient.Client;
using Xunit;

namespace AcceptanceTests.UseCases.AuthenticationRelated
{

    public class NonUsersPermissions : TimberyardTestCase
    {
        public NonUsersPermissions() : base()
        {
        }

        [Theory]
        [InlineData("", HttpStatusCode.BadRequest)]
        [InlineData("this is not valid email", HttpStatusCode.BadRequest)]
        [InlineData("thisIsNotValid.com", HttpStatusCode.BadRequest)]
        [InlineData("thisIsNotValid@comcom", HttpStatusCode.BadRequest)]
        [InlineData("thisIsNotValid@.com", HttpStatusCode.BadRequest)]
        [InlineData("someEmail@timberyard.rbbn.com", HttpStatusCode.OK)]
        public async Task ForgotPassword(string email, HttpStatusCode expectedStatusCode)
        {
            var forgotPasswordresponse = await Client.ForgetPassword(email);
            Assert.Equal(expectedStatusCode, forgotPasswordresponse.StatusCode);
        }

        [Theory]
        [InlineData("admin@timberyard.rbbn.com", "Password123!", HttpStatusCode.OK, true)]
        [InlineData("admin@timberyard.rbbn.com", "NotPassword!123", HttpStatusCode.OK, false)]
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
                var result = JsonConvert.DeserializeObject<Result<JWTToken>>(response.Content);
                Assert.Equal(expectedTokenResult, result.Status);
                Assert.Equal(expectedTokenResult, !string.IsNullOrEmpty(result.Data?.Token));
            }
        }

        [Fact]
        public async Task RequestVerificationcodeTest()
        {
            var response = await Client.RequestVerificationCode("differentRegularUser@timberyard.rbbn.com");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
