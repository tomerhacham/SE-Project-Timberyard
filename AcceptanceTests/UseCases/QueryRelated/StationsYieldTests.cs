using AcceptanceTests.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using Xunit;

namespace AcceptanceTests.UseCases.QueryRelated
{
    public class StationsYieldTests : TimberyardTestCase
    {
        public StationsYieldTests() : base()
        { }

        [Theory]
        [InlineData("OA_HF", "J", 2020, 2022, HttpStatusCode.OK, true)]          // Happy - catalog ID exists

        public async void GoodAcceptenceScenarioes(string station, string catalog, int start, int end, HttpStatusCode expectedStatusCode, bool isEmptyContent)
        {
            IRestResponse response = await Client.CalculateStationAndCardYield(station, catalog, new DateTime(start, 1, 10), new DateTime(end, 1, 10));
            Assert.Equal(expectedStatusCode, response.StatusCode);
            dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.Equal(isEmptyContent, content.Records > 0);
        }
    }
}
