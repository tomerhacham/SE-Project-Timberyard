using AcceptanceTests.Utils;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using TimberyardClient.Client;
using Xunit;

namespace AcceptanceTests.UseCases.AuthenticationRelated
{
    [Trait("Category", "Acceptance")]
    public class NonUsersPermissions : TimberyardTestCase
    {
        public NonUsersPermissions() : base()
        {
        }

        [Fact]
        public async Task ForgotPassword()
        {
            var addAdminResponse = await Client.ForgetPassword("someEmail@timberyard.rbbn.com");
            Assert.Equal(HttpStatusCode.OK, addAdminResponse.StatusCode);
        }
        [Theory]
        [InlineData("admin@timberyard.rbbn.com", "Password!123", HttpStatusCode.OK, true)]
        [InlineData("admin@timberyard.rbbn.com", "NotPassword!123", HttpStatusCode.NoContent, false)]
        [InlineData("regularUser@timberyard.rbbn.com", "NotPassword!123", HttpStatusCode.NoContent, false)]
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

        [Fact]
        public async Task RequestVerificationcodeTest()
        {
            var response = await Client.RequestVerificationCode("regularuser@timberyard.rbbn.com");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
