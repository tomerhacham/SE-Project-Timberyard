using AcceptanceTests.Utils;
using RestSharp;
using System;
using System.Net;
using Xunit;

namespace AcceptanceTests
{
    public class CardYieldTests : TimberyardTestCase
    {

        public CardYieldTests() : base()
        {

        }

        [Theory]
        [InlineData(2020, 2022, "OA_HF")]           // catalog ID exists
        [InlineData(2020, 2022, "OP_KLF")]
        public async void SuccessCaseTest(int start, int end, string catalog)
        {
            IRestResponse res = await Client.CalculateCardYield(catalog, new DateTime(start, 1, 10), new DateTime(end, 1, 10));
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        }

        [Theory]
        [InlineData(2020, 2022, "CCH1")]      // catalog ID not exists
        [InlineData(2020, 2022, "")]          // empty catalog ID
        [InlineData(2022, 2020, "")]          // start date > end date
        public async void NoExistCatalogNumberTest(int start, int end, string catalog)
        {
            IRestResponse res = await Client.CalculateCardYield(catalog, new DateTime(start, 1, 10), new DateTime(end, 1, 10));
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        }

    }
}
