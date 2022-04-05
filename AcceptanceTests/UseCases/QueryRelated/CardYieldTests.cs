using AcceptanceTests.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace AcceptanceTests
{
    public class CardYieldTests : TimberyardTestCase
    {

        public CardYieldTests() : base()
        { }

        [Theory]
        [InlineData(2020, 2022, "OA_HF", HttpStatusCode.OK, true)]          // Happy - catalog ID exists
        [InlineData(2020, 2022, "OP_KLF", HttpStatusCode.OK, true)]         // Happy - catalog ID exists
        [InlineData(2020, 2022, "CCH1", HttpStatusCode.OK, false)]            // Sad - catalog ID not exists
        public async void GoodAcceptenceScenarioes(int start, int end, string catalog, HttpStatusCode expectedStatusCode, bool isEmptyContent)
        {
            IRestResponse response = await Client.CalculateCardYield(catalog, new DateTime(start, 1, 10), new DateTime(end, 1, 10));
            Assert.Equal(expectedStatusCode, response.StatusCode);
            dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.Equal(isEmptyContent, content.Records > 0);
        }

        [Theory]
        [InlineData(2020, 2022, "", HttpStatusCode.BadRequest)]         // Bad - empty catalog ID
        [InlineData(2022, 2020, "SomeCatalog", HttpStatusCode.BadRequest)]         // Bad - start date > end date
        public async void BadAcceptenceScenarioes(int start, int end, string catalog, HttpStatusCode expectedStatusCode)
        {
            IRestResponse response = await Client.CalculateCardYield(catalog, new DateTime(start, 1, 10), new DateTime(end, 1, 10));
            Assert.Equal(expectedStatusCode, response.StatusCode);

        }

    }
}
