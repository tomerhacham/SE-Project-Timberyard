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
        public async Task LoginTest(string email, string password, HttpStatusCode expectedStatusCode,bool expectedTokenResult)
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
    }
}
